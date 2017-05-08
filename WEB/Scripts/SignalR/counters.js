$(function () {

    var dialog = $.connection.connectionHub;
    // Объявление функции, которая хаб вызывает при получении сообщений
    dialog.client.messageCounter = function (count) {
        $("#messageCounter").show();
        $("#messageCounter").text(count);
    };
    dialog.client.friendCounters = function (count) {
        if (count > 0) {
            $("#friendsCounter").show();
            $("#friendsCounter").text(count[0]);
        }
    };
    //dialog.client.friendCounters = function (count) {
    //    $("#followersCounter").show();
    //    $("#followersCounter").text(count[0]);
    //};
    //dialog.client.friendCounters = function (count) {
    //    $("#followedCounter").show();
    //    $("#followedCounter").text(count[2]);
    //};
    $.connection.hub.start();

});
