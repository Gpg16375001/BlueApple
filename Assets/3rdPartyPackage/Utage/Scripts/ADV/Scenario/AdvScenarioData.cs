// UTAGE: Unity Text Adventure Game Engine (c) Ryohei Tokimura
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UtageExtensions;


namespace Utage
{

	/// <summary>
	/// シナリオのデータ
	/// </summary>
	public class AdvScenarioData
	{
		/// <summary>
		/// シナリオ名
		/// </summary>
		string Name { get { return this.name; } }
		string name;

		//グリッドデータ
		public AdvImportScenarioSheet DataGrid{ get; private set; }

		/// <summary>
		/// データグリッド名
		/// </summary>
		public string DataGridName { get { return DataGrid.Name; } }

		/// <summary>
		/// 初期化済みか
		/// </summary>
		public bool IsInit { get { return this.isInit; } }
		bool isInit = false;

		/// <summary>
		/// バックグランドでのロード処理を既にしているか
		/// </summary>
		public bool IsAlreadyBackGroundLoad { get { return this.isAlreadyBackGroundLoad; } }
		bool isAlreadyBackGroundLoad = false;
	
		/// <summary>
		/// このシナリオからリンクするジャンプ先のシナリオラベル
		/// </summary>
		public List<AdvScenarioJumpData> JumpDataList { get { return this.jumpDataList; } }
		List<AdvScenarioJumpData> jumpDataList = new List<AdvScenarioJumpData>();
/*
		/// <summary>
		/// このシナリオ内のページデータ
		/// </summary>
		public List<AdvScenarioLabelData> ScenarioLabelDataList { get { return this.scenarioLabelDataList; } }
		List<AdvScenarioLabelData> scenarioLabelDataList = new List<AdvScenarioLabelData>();
*/
		/// <summary>
		/// このシナリオ内のページデータ
		/// </summary>
		public Dictionary<string,AdvScenarioLabelData> ScenarioLabels { get { return this.scenarioLabels; } }
		Dictionary<string, AdvScenarioLabelData> scenarioLabels = new Dictionary<string, AdvScenarioLabelData>();

		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="name">シナリオ名</param>
		/// <param name="grid">シナリオデータ</param>
		public AdvScenarioData(AdvImportScenarioSheet grid)
		{
			this.name = grid.SheetName;
			this.DataGrid = grid;
		}

		//シナリオデータとして解析
		public void Init(AdvSettingDataManager dataManager)
		{
			isInit = false;
			List<AdvCommand> commandList = DataGrid.CreateCommandList(dataManager);

			Profiler.BeginSample("選択肢終了などの特別なコマンドを自動解析して追加");
			//選択肢終了などの特別なコマンドを自動解析して追加
			AddExtraCommand(commandList, dataManager);
			Profiler.EndSample();

            //_/_/_/_/_/_/_/ 以下自装ロジック. 選択肢設定時のID設定チェック. by yoshida ryo /_/_/_/_/_/_/_/
            Profiler.BeginSample("選択肢IDのバリデーションチェック");
            CheckValidationSelectID(commandList);
            Profiler.EndSample();
            //_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/

			Profiler.BeginSample("シナリオラベルデータを作成");
			//シナリオラベルデータを作成
			MakeScanerioLabelData(commandList);
			Profiler.EndSample();

			Profiler.BeginSample("このシナリオからリンクするジャンプ先のシナリオラベルを取得");
			//このシナリオからリンクするジャンプ先のシナリオラベルを取得
			MakeJumpDataList(commandList);
			Profiler.EndSample();
			isInit = true;
		}

		/// <summary>
		/// 選択肢終了などの特別なコマンドを自動解析して追加
		/// </summary>
		/// <param name="continuousCommand">連続しているコマンド</param>
		/// <param name="currentCommand">現在のコマンド</param>
		void AddExtraCommand(List<AdvCommand> commandList, AdvSettingDataManager dataManager)
		{
			int index = 0;
			while (index < commandList.Count)
			{
				AdvCommand current = commandList[index];
				AdvCommand next = index +1 < commandList.Count ? commandList[index+1] : null;
				++index;
				string[] idArray = current.GetExtraCommandIdArray(next);
				if (idArray!=null)
				{
					foreach(string id in idArray)
					{
						AdvCommand extraCommand = AdvCommandParser.CreateCommand(id, current.RowData, dataManager);
						if (current.IsEntityType)
						{
							extraCommand.EntityData = current.EntityData;
						}
						commandList.Insert(index, extraCommand);
						++index;
					}
				}
			}
		}

        /// <summary>
        /// 以下自装ロジック. 選択肢設定時のID設定チェック. by yoshida ryo
        /// 選択肢設定時のID設定チェック.
        /// </summary>
        void CheckValidationSelectID(List<AdvCommand> commandList)
        {
            var list = AdvSelectInfoEx.ReadData();
            selectIdList.Clear();

            for (var i = 0; i < commandList.Count; ++i) {
                var cmd = commandList[i];

                // SendMessageByNameによるID送信チェック.
                if (cmd.GetType().ToString().Contains("SendMessageByName")) {
                    string receiver = cmd.ParseCellOptional("Arg1", "");
                    string method = cmd.ParseCellOptional("Arg2", "");
                    if (receiver == "ReceiveUtageCommand" && method == "RegistSelectionID") {
                        int id = -1;
                        if (!cmd.TryParseCell("Arg3", out id)) {
                            continue;
                        }
                        var lineNo = cmd.RowData.DebugIndex + 1;
                        // 別のシートで重複.
                        if(list.Exists(sel => sel.ID == id)){
                            var item = list.Find(sel => sel.ID == id);
                            var msg = "<color=red>『"+Name+"』シート:『"+lineNo+"』行目で使用されている選択肢ID『" + id + "』 は既に別のシートで使用済みのIDです。";
                            msg += "\n→" + item.SheetName + "シートの" + item.RowIndex + "行目で使用してます。</color>";
                            Debug.LogError(cmd.RowData.ToErrorString(msg));
                            continue;
                        }
                        // 今回の修正で重複した.
                        if (selectIdList.ContainsKey(id)) {
                            var msg = "<color=red>『"+Name+"』シート:『"+lineNo+"』行目で使用されている選択肢ID『" + id + "』 は既にこのシートで使用済みのIDです。";
                            msg += "\n→" + selectIdList[id] + "行目で使用してます。</color>";
                            Debug.LogError(cmd.RowData.ToErrorString(msg));
                            continue;
                        }
                        AdvSelectInfo info;
                        info.SheetName = Name;
                        info.RowIndex = lineNo;
                        info.ID = id;
                        info.WriteData();
                        selectIdList.Add(id, lineNo);
                    }
                }
            }
        }
        private Dictionary<int/*id*/, int/*lineNo*/> selectIdList = new Dictionary<int, int>();

		//シナリオラベル区切りのデータを作成
		void MakeScanerioLabelData(List<AdvCommand> commandList)
		{
			if (commandList.Count <= 0) return;

			//最初のラベル区切りデータは自身の名前（シート名）
			string scenarioLabel = Name;
			AdvCommandScenarioLabel scenarioLabelCommand = null;
			AdvScenarioLabelData scenarioLabelData = null;
			int commandIndex = 0;
			do
			{
				int begin = commandIndex;
				//コマンドを追加
				while (commandIndex < commandList.Count)
				{
					//シナリオラベルがあるので、終了
					if (commandList[commandIndex] is AdvCommandScenarioLabel)
					{
						break;
					}
					++commandIndex;
				}

				if (IsContainsScenarioLabel(scenarioLabel))
				{
					//重複してないかチェック
					Debug.LogError(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.RedefinitionScenarioLabel, scenarioLabel, DataGridName));
				}
				else
				{
					//ラベルデータ追加
					AdvScenarioLabelData next = new AdvScenarioLabelData(scenarioLabel, scenarioLabelCommand, commandList.GetRange(begin, commandIndex - begin));
					if (scenarioLabelData != null)
					{
						scenarioLabelData.Next = next;
					}
					scenarioLabelData = next;
					scenarioLabels.Add(scenarioLabel, next);
				}

				if (commandIndex >= commandList.Count)
				{
					break;
				}

				scenarioLabelCommand = commandList[commandIndex] as AdvCommandScenarioLabel;
				scenarioLabel = scenarioLabelCommand.ScenarioLabel;
				++commandIndex;
			} while (true);
		}


		//このシナリオからリンクするジャンプ先のシナリオラベルを取得
		void MakeJumpDataList(List<AdvCommand> commandList)
		{
			JumpDataList.Clear();
			commandList.ForEach(
				command =>
				{
					///このシナリオからリンクするジャンプ先のシナリオラベルを取得
					string[] jumpLabels = command.GetJumpLabels();
					if (jumpLabels != null)
					{
						foreach (var jumpLabel in jumpLabels)
						{
							JumpDataList.Add(new AdvScenarioJumpData(jumpLabel, command.RowData));
						}
					}
				});
		}


#if UNITY_EDITOR
		// 文字数オーバー　チェック
		public int EditorCheckCharacterCount(AdvEngine engine, Dictionary<string, AdvUguiMessageWindow> windows)
		{
			int count = 0;
			foreach (var keyValue in ScenarioLabels)
			{
				count += keyValue.Value.EditorCheckCharacterCount(engine,windows);
			}
			return count;
		}
#endif

		/// <summary>
		/// バックグランドでダウンロードだけする
		/// </summary>
		/// <param name="dataManager">各種設定データ</param>
		public void Download(AdvDataManager dataManager)
		{
			foreach (var keyValue in ScenarioLabels)
			{
				keyValue.Value.Download(dataManager);
			}
			isAlreadyBackGroundLoad = true;
		}

		/// <summary>
		/// 指定のシナリオラベルがあるかチェック
		/// </summary>
		/// <param name="scenarioLabel">シナリオラベル</param>
		/// <returns>あったらtrue。なかったらfalse</returns>
		public bool IsContainsScenarioLabel(string scenarioLabel)
		{
			return FindScenarioLabelData(scenarioLabel) != null;
		}

		/// <summary>
		/// 指定のシナリオラベルがあるかチェック
		/// </summary>
		/// <param name="scenarioLabel">シナリオラベル</param>
		/// <returns>あったらtrue。なかったらfalse</returns>
		public AdvScenarioLabelData FindScenarioLabelData(string scenarioLabel)
		{
			return ScenarioLabels.GetValueOrGetNullIfMissing(scenarioLabel);
		}

		public AdvScenarioLabelData FindNextScenarioLabelData(string scenarioLabel)
		{
			AdvScenarioLabelData current = FindScenarioLabelData(scenarioLabel);
			if (current != null)
			{
				return current.Next;
			}
			else
			{
				return null;
			}
		}
	}
}