function WebServer(mgr)
{
    var _this = this;
    // 创建一个Socket实例
    this.socket = null;

    this.run = function ()
    {
        if (typeof navigator.msLaunchUri != 'undefined')
        {
            console.log(mgr.Config.edge.protocol + "://" + mgr.Config.edge.port);
            //up6://9006
            navigator.msLaunchUri(mgr.Config.edge.protocol+"://"+mgr.Config.edge.port, function ()
            {
                console.log('应用打开成功');
                _this.socket = _this.connect();//
                //alert("success");
            }, function ()
            {
                console.log('启动失败');
            });
        }
    };
    this.connect = function ()
    {
        var socket = new WebSocket('ws://127.0.0.1:' + mgr.Config.edge.port);
        console.log("开始连接服务:" + 'ws://127.0.0.1:' + mgr.Config.edge.port);

        // 打开Socket 
        socket.onopen = function (event)
        {
            console.log("服务连接成功");
            // 发送一个初始化消息
            //socket.send('I am the client and I\'m listening!');

            // 监听消息
            socket.onmessage = function (event)
            {
                mgr.recvMessage(event.data);
                //console.log('Client received a message', event);
            };

            // 监听Socket的关闭
            socket.onclose = function (event)
            {
                //console.log('Client notified socket has closed', event);
            };

            // 关闭Socket.... 
            //socket.close() 
        };
        socket.onerror = function (event) { console.log("连接失败"); };
        return socket;
    };
    this.close = function ()
    {
        if (this.socket) { this.socket.close(1000,"close");}
    };
    this.send = function (p)
    {
        if(this.socket)this.socket.send(JSON.stringify(p));
    };
}