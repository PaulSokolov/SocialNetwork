$(function () {

    var dialog = $.connection.connectionHub;
    // Объявление функции, которая хаб вызывает при получении сообщений
    dialog.client.messageCounter = function (count) {
        $("#messageCounter").show();
        $("#messageCounter").text(count);
    };
    dialog.client.friendCounters = function(count) {
        //if (count.friends > 0) {
        //    $("#friendsCounter").show();
        //    $("#friendsCounter").text(count.friends);
        //} else
        //    $("#friendsCounter").hide();
        if (count.followers > 0) {
            $("#followersCounter").show();
            $("#followersCounter").text(count.followers);
        } else
            $("#followersCounter").hide();
        if (count.followed > 0) {
            $("#followedCounter").show();
            $("#followedCounter").text(count.followed);
        } else
            $("#followedCounter").hide();
        if (count.newfriends > 0) {
            $("#friendsCounter").show();
            $("#friendsCounter").text(count.newfriends);
        } else
            $("#friendsCounter").hide();
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
