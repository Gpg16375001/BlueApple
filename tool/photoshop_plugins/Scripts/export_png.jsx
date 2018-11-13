// enable double clicking from the Macintosh Finder or the Windows Explorer
#target photoshop

// in case we double clicked the file
app.bringToFront();

app.preferences.rulerUnits = Units.PIXELS;	// 単位をピクセルに
var docRef = app.activeDocument;
var baseURL = String(File(docRef.path).fsName).replace(/\\/g, "/" )+"/";
var currentFolder = "";

main();

function main()
{
    createFolder(getNameRemovedExtendType(docRef));
    
    setVisible(docRef.layers, false);

    var layerCount = docRef.layers.length;
    for(var i = 0; i < layerCount; ++i) {
        layerOutput(docRef.layers[i]);
    }
}

function layerOutput(layer) {
    if(layer.name.indexOf("#") === 0 ) {
        // #の付いてるレイヤーは無視するよ。
        return;
    }

    if(layer.typename == "ArtLayer") {
        // 普通のレイヤーはそのまま出力する
        if(layer.name.indexOf("txt_") == 0 || layer.name.indexOf("txtp_") == 0) {
            // テキストとしてコンバート予定のレイヤーは無視する。
            return;
        }
        clippingLayer(layer);
    }
    else if(layer.typename == "LayerSet"){
        if(layer.name.indexOf("bt_") === 0 || layer.name.indexOf("img_") === 0) {
            // ここはこのまま出力する。
            clippingLayer(layer);
        }
        else {
            // 下を再帰的に処理する。
            var layerCount = layer.layers.length;
            for(var i = 0; i < layerCount; ++i) {
                layerOutput(layer.layers[i]);
            }
        }
    }
}

//フォルダ作成処理
function createFolder( folderName ) {
	currentFolder += getValidName(folderName)+"/";

	_createFolder(baseURL+currentFolder);
}

function _createFolder(url) {
	var folder = new Folder(url);
	
	if( folder.exists ) {
		return false;
	}
	else {
		folder.create();
		return true;
	}
}
function getNameRemovedExtendType(doc) {
	var nameParts = String(doc.name).split(".");
	var name = nameParts.splice(0, nameParts.length-1).join(".");
	return name;
}
function getValidName(name){
    name = name.replace(/\/$/,"");
    var names = name.split('_');
    var nameslength = names.length;
    var newNames = [];
    if(nameslength > 1) {
        var startIndex = 1;
        newNames.push(names[0]);
        if(names[0] == 'img' || names[0] == 'bt' ) {
            newNames.push(names[1]);
            startIndex = 2;
        }

        for(var i = startIndex; i < nameslength; ++i) {
            if(names[i] == 'highlight' || names[i] == 'disable' || names[i] == 'pressed') {
                newNames.push(names[i]);
            }
        }
        name = newNames.join('_');
    }
	return name.replace(/[\/\:\;\.\,\@\"\'\\]/g,"_");
}

// レイヤー表示処理
function setVisible(obj, bool){
	var i=0, l;
	switch( obj.typename ) {
		//case "LayerSets":
		case "Layers":
			for( l=obj.length; i<l; ++i ) {
				setVisible(obj[i],bool);
			}
		break;
        case "LayerSet":
            // 除外レイヤー以下は問答無用で非表示
            if(obj.name.indexOf("#") === 0 ) {
                bool = false;
            }
			obj.visible = bool;
			for( l=obj.layers.length; i<l; ++i ) {
				setVisible(obj.layers[i], bool);
			}
		break;
        default:
            // 除外レイヤー以下は問答無用で非表示
            if(obj.name.indexOf("#") === 0 ) {
                bool = false;
            }
			obj.visible = bool;
			if( bool ) displayParent( obj );
		break;
	}
}
function displayParent(obj){
    if(obj.parent){
        obj.parent.visible = true;
        displayParent( obj.parent );
    }
}
function isLayerSet(obj){
	return Boolean(obj.typename == "LayerSet");
}

function cTID(s) {return app.charIDToTypeID(s);}
function sTID(s) {return app.stringIDToTypeID(s);}

function selectLayer(layer) {
	var desc = new ActionDescriptor();
    var ref = new ActionReference();
    ref.putEnumerated(cTID("Chnl"), cTID("Chnl"), cTID("Msk "));
	ref.putName(cTID("Lyr "), layer.name );
	desc.putReference(cTID("null"), ref);
	executeAction( cTID("slct"), desc, DialogModes.NO );
}

function hasLayerMask() {
	var hasLayerMask = false;
	try {
		var ref = new ActionReference();
		var keyUserMaskEnabled = cTID( 'UsrM' );
		ref.putProperty( cTID( 'Prpr' ), keyUserMaskEnabled );
		ref.putEnumerated( cTID( 'Lyr ' ), cTID( 'Ordn' ), cTID( 'Trgt' ) );
		var desc = executeActionGet( ref );
		if ( desc.hasKey( keyUserMaskEnabled ) ) {
			hasLayerMask = true;
		}
	}catch(e) {
		hasLayerMask = false;
	}
	return hasLayerMask;
}

function addSelection() {
    var desc = new ActionDescriptor();
    var ref = new ActionReference();
    ref.putEnumerated(cTID("Chnl"), cTID("Ordn"), cTID("Trgt"));
    var refT = new ActionReference();
    refT.putProperty( cTID("Chnl"), cTID("fsel") );
    desc.putReference( cTID("null"), refT );
    desc.putReference( cTID("T   "), ref );
    desc.putInteger( cTID("Vrsn"), 1 );
    desc.putBoolean( sTID("maskParameters"), true );
    executeAction( cTID("setd"), desc, DialogModes.NO );
}

function selectMaskRenge(layer) {
    try {
        selectLayer(layer);
        if(hasLayerMask() == true) {
            addSelection();
            return true;
        }
    } catch(e) {
    }
    return false;
}

// メイン処理レイヤー
function clippingLayer(obj){
    var name = getValidName(obj.name);
    if(isExistFile(currentFolder, name, "png")) {
        return;
    }
    //書き出し準備
    setVisible(docRef.layers, false);
    setVisible(obj, true);

    // TODO: あとで見切れないようにレイヤーを移動する処理が必要になるかも
    
    app.activeDocument = docRef;
    docRef.activeLayer = obj;

    var width = 0;
    var height = 0;
    var left = 0;
    var top = 0;
    var right = 0;
    var bottom = 0;
    var maskUsed = false;
    if(selectMaskRenge(obj) == true) {
        var maskBoundsObj = docRef.selection.bounds;
        var mask_x1 = parseInt(maskBoundsObj[0]);
        var mask_y1 = parseInt(maskBoundsObj[1]);
        var mask_x2 = parseInt(maskBoundsObj[2]);
        var mask_y2 = parseInt(maskBoundsObj[3]);

        if(mask_x1 < 0) mask_x1 = 0;
        if(mask_y1 < 0) mask_y1 = 0;
        if(mask_x2 > docRef.width) mask_x2 = docRef.width;
        if(mask_y2 > docRef.height) mask_x2 = docRef.height;
        width = mask_x2 - mask_x1;
        height = mask_y2 - mask_y1;

        //レイヤーの画像範囲を取得
        var boundsObj = obj.bounds;
        var x1 = parseInt(boundsObj[0]);
        var y1 = parseInt(boundsObj[1]);
        var x2 = parseInt(boundsObj[2]);
        var y2 = parseInt(boundsObj[3]);

        if(x1 < 0) x1 = 0;
        if(y1 < 0) y1 = 0;
        if(x2 > docRef.width) x2 = docRef.width;
        if(y2 > docRef.height) y2 = docRef.height;

        left = x1 - mask_x1;
        top = y1 - mask_y1;
        right = left + (x2 - x1);
        bottom = top + (y2 - y1);

        //指定範囲を選択
        var selectReg = [[x1,y1],[x2,y1],[x2,y2],[x1,y2]];
        docRef.selection.select(selectReg);

        maskUsed = true;
    }
    else {
        //レイヤーの画像範囲を取得
        var boundsObj = obj.bounds;
        var x1 = parseInt(boundsObj[0]);
        var y1 = parseInt(boundsObj[1]);
        var x2 = parseInt(boundsObj[2]);
        var y2 = parseInt(boundsObj[3]);

        if(x1 < 0) x1 = 0;
        if(y1 < 0) y1 = 0;
        if(x2 > docRef.width) x2 = docRef.width;
        if(y2 > docRef.height) y2 = docRef.height;
        width = x2 - x1;
        height = y2 - y1;

        left = 0;
        top = 0;
        right = width;
        bottom = height;
        
        //指定範囲を選択
        var selectReg = [[x1,y1],[x2,y1],[x2,y2],[x1,y2]];
        docRef.selection.select(selectReg);
    }

    try {
        //選択範囲を結合してコピー
        docRef.selection.copy(true);

        //選択を解除
        docRef.selection.deselect();

        //新規ドキュメントを作成
        var resolution = 72;
        var mode = NewDocumentMode.RGB;
        var initialFill = DocumentFill.TRANSPARENT;

        preferences.rulerUnits = Units.PIXELS;
        newDocument = documents.add(width, height, resolution, name, mode, initialFill);

        if(maskUsed == true) {
            //ペースト範囲を選択
            var selectReg = [[left,top],[right,top],[right,bottom],[left,bottom]];
            newDocument.selection.select(selectReg);
        }

        //画像をペースト
        newDocument.paste();
        newDocument.selection.deselect();

        app.activeDocument = newDocument;
        //ファイルに書き出し
        savePNG( currentFolder, name );

        newDocument.close( SaveOptions.DONOTSAVECHANGES );
	}
	catch(e){
        //選択範囲に何も含まれていない場合
        alert(e.message);
        newDocument.close( SaveOptions.DONOTSAVECHANGES );
	}
	finally{
        newLayer.remove();

		//元のドキュメントをアクティブに設定
		app.activeDocument = docRef;
		setVisible(docRef.layers, false);
	}
}

// 保存処理
function savePNG(path, name) {
	var exp = new ExportOptionsSaveForWeb();
	exp.format = SaveDocumentType.PNG;
	exp.interlaced　= false;
    exp.PNG8 = false;

	fileObj = new File( getFileName(path, name, "png") );
	app.activeDocument.exportDocument(fileObj, ExportType.SAVEFORWEB, exp);
	
	return fileObj.name;
}

function isExistFile(path, name, ext) {
	path = baseURL + path;
    
    name = name.replace(/_left/g, "");
    name = name.replace(/_right/g, "");
    name = name.replace(/_top/g, "");
    name = name.replace(/_bottom/g, "");
    var filename = [ path, name ].join("/");
    var count = 0;
    var newFileName = "";

    newFileName = filename + "." + ext
    var file = new File(newFileName);
	return file.exists;
}

// ファイル名の重複回避処理
function getFileName( path, name, ext ) {
	path = baseURL + path;

    name = name.replace(/_left/g, "");
    name = name.replace(/_right/g, "");
    name = name.replace(/_top/g, "");
    name = name.replace(/_bottom/g, "");
	var filename = [ path, name ].join("/");
	var count = 0;
	var newFileName = "";

	newFileName = filename + "." + ext
	
	var file = new File(newFileName);
	while(file.exists != false){
		count +=1;
		newFileName = filename + count + "." + ext
		file = new File(newFileName);
	}
	return newFileName;
}