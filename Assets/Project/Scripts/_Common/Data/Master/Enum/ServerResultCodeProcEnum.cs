
public enum ServerResultCodeProcEnum {
    None = 1, // 何もしない
    Reboot = 2, // タイトルに戻す
    MoveStore = 3, // ストアに遷移
    Maintenance = 4, // メンテナンス画面を表示
    HealAP = 5, // AP回復を表示
    HealBP = 6, // BP回復を表示
    GemShop = 7, // ジェム購入を表示
}
