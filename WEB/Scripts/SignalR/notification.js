$(function () {
        
    var dialog = $.connection.connectionHub;
    // Объявление функции, которая хаб вызывает при получении сообщений
    dialog.client.notification = function (notification) {
        $("#messageNotification").replaceWith(notification);
        $("#messageNotification").show();
        $("#closeMessageNotification").on("click", function () {
            $("#messageNotification").hide();
        });
        
    };
    dialog.client.addFriend = function (notification) {
        $("#friendNotification").replaceWith(notification);
        $("#friendNotification").show();
        $("#closeFriendNotification").on("click", function () {
            $("#friendNotification").hide();
        });
    }
    $.connection.hub.start();
    
});
