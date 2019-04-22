/**
 * 搜索组合器
 * 使用方法：
 * var ss = new SqlSearch({key:{},ui:{panel:"",record:""},event:{change:function(sql){}}});
 * 添加搜索条件：add_condition(显示名称，字段名称，表达式)
 * html写法：
 *  <a sql-field="assigned" sql-expression="'assigned='+m.name+"">显示名称</a>
 * vue写法：
 *  <a v-for="m in members" sql-field="assigned" class="m-r-md link" :sql-expression="'assigned=\''+m.name+'\''">{{m.name_cn}}</a>
 * @param {any} obj
 */
function SqlSearch(obj)
{
    var _this = this;
    this.sett = {
        btn_key:"key"//与查询表关联的属性名称
        , key: {}//搜索条件表{state0: {name: "待处理", field: "state", expression: "state=0"},}
        ,key_ex:[]//动态添加的条件，
        , ui: {
            panel: ""//搜索操作面板
            , record: ""//搜索记录
        }
        , link_css: "btn btn-link p-0 m-r-md border-0 text-size-xs"
        , record: []//搜索记录(数组)
        , event: {
            change: function (sql) { }
        }
    };
    jQuery.extend(this.sett, obj);
    if (typeof (page.where) != "undefined") this.sett.record = page.where;

    //初始化,查询表，搜索记录
    this.init = function () {

        //初始化表
        this.bind_ui(this.sett.ui.panel);
        //初始化搜索记录
        this.init_record(this.sett.ui.record);
    };

    //与链接关联，绑定点击事件
    this.bind_ui = function (id) {
        var obj = $(id);

        //支持从标签中动态绑定属性
        $.each(obj.find("a[sql-field]"), function (i, n) {
            var ui = $(this);
            var k = ui.attr("sql-field");
            var exp = ui.attr("sql-expression");
            var name = ui.text();
            var name_f = k + i;
            _this.sett.key_ex.push( { "name": name, expression: exp, field: k } );
        });

        obj.find("a").addClass(_this.sett.link_css).bind("click", function () {
            var name = $(this).attr(_this.sett.btn_key);
            //动态提取的属性
            if ($(this)[0].hasAttribute("sql-field"))
            {
                _this.add_condition($(this).text()
                    , $(this).attr("sql-field")
                    , $(this).attr("sql-expression")
                )
            }
            else {
                _this.add_con(name);
            }
            _this.init_record(_this.sett.ui.record);
            _this.sett.event.change(_this.to_sql());
        });
    };

	//初始化UI,解析服务器搜索记录值：this.sett.record
    this.init_record = function (id) {
        var html = ' ';
        var ico = '<span class="glyphicon glyphicon-remove" aria-hidden="true"></span>';
        var lis = [];
        if (typeof (this.sett.record) != "undefined" && this.sett.record.length>0) {
            for (var i = 0; i < this.sett.record.length; ++i) {
                var li = "<a class='link m-r-sm' field='" + this.sett.record[i].field + "'>" + ico + this.sett.record[i].name +"</a>";
                lis.push(li);
            }
            if (this.sett.record.length > 0) {
                lis.push("<a class='link m-r-sm' field='clear-all'>"+ico+"清除所有记录</a>");
            }
            html = lis.join("");
        }
        var obj = $(id).html(html);
        obj.find("a").bind("click", function () {
            var fn = $(this).attr("field");

            if (fn == "clear-all") {
                _this.sett.record.length = 0;
                obj.empty();
            }
            else {
                _this.del_condition(fn);
                //删除当前项
                $(this).remove();
            }
            
            _this.sett.event.change(_this.to_sql());
        });
	};

	//将搜索条件转换成json串,
    this.to_sql = function () {
        return JSON.stringify(this.sett.record);
	};

    this.search = function (n, f, e) {
        this.add_condition(n, f, e);
        _this.init_record(_this.sett.ui.record);
        _this.sett.event.change(_this.to_sql());
    };

	/*
		添加搜索条件
		n:显示名称
		f:字段名称
		e:表达式。a=0
	*/
    this.add_condition = function (n, field, e) {
        if (this.sett.record.length > 0) {
            //过滤相同字段
            this.sett.record = $.grep(this.sett.record, function (n, i) {
                return n.field == field;
            }, true);
        }
        //
        this.sett.record.push({ name: n, "field": field, expression:e});
    };

    this.add_con = function(name)
    {
        var obj = this.sett.key[name];
        //过滤相同字段
        this.sett.record = $.grep(this.sett.record, function (n, i) {
            return n.field == obj.field;
        }, true);
        //
        this.sett.record.push(obj);

        //this.sett.event.change(this.to_sql());
    };

    //删除搜索条件字段
    this.del_condition = function (f) {
        this.sett.record = $.grep(this.sett.record, function (n, i) {
            return n.field == f;
        }, true);
    };

    //初始化
    this.init();
}