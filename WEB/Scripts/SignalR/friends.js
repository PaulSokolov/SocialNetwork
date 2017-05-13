$(function () {

    $("#getFriends").click(function () {
        ajaxGetUsersRequest("Friends");
    });
    $("#followers").click(function (e) {
        ajaxGetUsersRequest("Followers");
    });
    $("#followed").click(function () {
        ajaxGetUsersRequest("Followed");
    });
    var hub = $.connection.connectionHub;

    hub.client.addFriend = function (publicId) {

        $('#friend' + publicId).replaceWith("Added");
        scrollMessages();
    };

    $.connection.hub.start();
    
});

function ajaxGetUsersRequest(action) {
    $.ajax({
        type: "POST",
        url: "/Friend/"+action,
        dataType: "html",
        success: function (data) {
            $("#result").empty().append(data);
        },
        beforeSend: function () { beforeSend('result') },
        complete: function () { complete('result') },
        failure: function () {
            alert("error");
        }
    })
}


function FriendRequest(publicId, action, elementId, waitId) {
    var id = publicId;
    var data = { publicId: id };
    var url = "/Friend/" + action;
    $.ajax({
        url: url,
        data: data,
        type: "post",
        dataType: 'html',
        success: function (data) {
            //$('#' + elementId + '_' + publicId).replaceWith(data);
            if (data === 'Add To Friends') {
                var newId = 'follower';
                $('#' + elementId + '_' + publicId).removeClass('btn-danger');
                $('#' + elementId + '_' + publicId).attr('onclick', 'FriendRequest(' + publicId + ',\'Confirm\',\'' + newId + '\',\'' + waitId +'\')');
                $('#' + elementId + '_' + publicId).text(data);
                $('#' + elementId + '_' + publicId).attr('id', 'follower_' + publicId);
            }
            else if (data === 'Delete') {
                var newId = 'delete';
                $('#' + elementId + '_' + publicId).addClass('btn-danger');
                $('#' + elementId + '_' + publicId).attr('onclick', 'FriendRequest(' + publicId + ',\'Delete\',\'' + newId + '\',\'' + waitId +'\')');
                $('#' + elementId + '_' + publicId).text(data);
                $('#' + elementId + '_' + publicId).attr('id', newId + '_' + publicId);
            }
            else if (data === 'Unsubscribe') {
                var newId = 'followed'
                $('#' + elementId + '_' + publicId).addClass('btn-danger');
                $('#' + elementId + '_' + publicId).attr('onclick', 'FriendRequest(' + publicId + ',\'Unsubscribe\',\'' + newId + '\',\'' + waitId +'\')');
                $('#' + elementId + '_' + publicId).text(data);
                $('#' + elementId + '_' + publicId).attr('id',  newId + '_' + publicId);
            }
            else if (data === 'Add Friend') {
                var newId = 'friend'
                $('#' + elementId + '_' + publicId).removeClass('btn-danger');
                $('#' + elementId + '_' + publicId).attr('onclick', 'FriendRequest(' + publicId + ',\'Add\',\'' + newId + '\',\'' + waitId +'\')');
                $('#' + elementId + '_' + publicId).text(data);
                $('#' + elementId + '_' + publicId).attr('id', newId + '_' + publicId);
            }

        },
        beforeSend: function () { beforeSend(waitId) },
        complete: function () { complete(waitId) }
    });
}
