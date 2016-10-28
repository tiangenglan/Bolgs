$(function(){
	$("#tabs").tabs({
            onSelect:function(title,index){
                //alert("出来啦！"+title+ index);
            }
        }); 

	InitLeftMenu();
	tabClose();
	
})

//初始化左侧
function InitLeftMenu() {

    var menulist = '';

    $.each(_menus.menus, function(i, n) {
        menulist  = '<div title="'+n.menuname+'"  data-options="iconCls:\''+n.icon+'\'" style="overflow:auto;font-size:14px;">';
		menulist += '<ul>';
        $.each(n.menus, function(j, o) {
			menulist += '<li><div><a style="position:relative;padding-left:20px;" target="mainFrame" way="' + o.url + '" ><span style="position:absolute;left:0px;margin-top:3px;" class="icon '+o.icon+'" ></span>' + o.menuname + '</a></div></li> ';
        })
        menulist += '</ul>';
        menulist += '</div>';

        $('.easyui-accordion').accordion('add', {
			title: n.menuname,
			content:menulist,
			iconCls:n.icon,
			selected: n.selected
		});
    })

	
	$('.easyui-accordion li a').click(function(){
		var tabTitle = $(this).text();
		var url = $(this).attr("way");
		//alert(tabTitle);
		addTab(tabTitle,url);
		$('.easyui-accordion li div').removeClass("selected");
		$(this).parent().addClass("selected");
	}).hover(function(){
		$(this).parent().addClass("hover");
	},function(){
		$(this).parent().removeClass("hover");
	});

}

function addTab(subtitle,url){

	if ($('#tabs').tabs('exists', subtitle)) {
        $('#tabs').tabs('close', subtitle);
    }

	//alert(subtitle+url);
	if(!$('#tabs').tabs('exists',subtitle)){
		$('#tabs').tabs('add',{
			title:subtitle,
			content:createFrame(url),
			closable:true,
			width:$('#mainPanle').width()-10,
			height:$('#mainPanle').height()-26
		});
		return false;
	}else{
		$('#tabs').tabs('select',subtitle);
		return false;
	}

	tabClose();
}

function createFrame(url)
{
	var s = '<iframe name="mainFrame" scrolling="auto" frameborder="0"  src="'+url+'" style="width:100%;height:100%;"></iframe>';
	return s;
}

function tabClose()
{
	/*双击关闭TAB选项卡*/
	$(".tabs-inner").dblclick(function(){
		var subtitle = $(this).children("span").text();
		$('#tabs').tabs('close',subtitle);
	})

}


//弹出信息窗口 title:标题 msgString:提示信息 msgType:信息类型 [error,info,question,warning]
function msgShow(title, msgString, msgType) {
	$.messager.alert(title, msgString, msgType);
}
/**
* 更换EasyUI主题的方法
* @param themeName
* 主题名称
*/
changeTheme = function(themeName) {
var $easyuiTheme = $('#easyuiTheme');
var url = $easyuiTheme.attr('href');
var href = url.substring(0, url.indexOf('themes')) + 'themes/' + themeName + '/easyui.css';
$easyuiTheme.attr('href', href);
var $iframe = $('iframe');
if ($iframe.length > 0) {
for ( var i = 0; i < $iframe.length; i++) {
var ifr = $iframe[i];
$(ifr).contents().find('#easyuiTheme').attr('href', href);
}
}
$.cookie('easyuiThemeName', themeName, {
expires : 7
});
};