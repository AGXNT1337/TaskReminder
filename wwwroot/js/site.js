$(document).ready(function () {
    fetchNotifications();
    function fetchNotifications() {
        $.ajax({
            url: '/Tasks/GetNotifications',
            type: 'GET',
            dataType: 'json',
            success: function (notifications) {
                // Update UI with the latest notifications
                $('#notificationContainer').empty(); // Clear existing notifications
                notifications.forEach(function (notification) {
                    if (!notification.playedSound) {
                        playNotificationSound();
                        notificationSoundPlayed(notification.id);
                    }

                    // Append notification with close button
                    var $notification = $('<div class="notification">' +
                        '<span class="message">' + notification.message + '</span>' +
                        '<button class="closeButton"><i class="bi bi-x-lg"></i></button>' +
                        '</div>');
                    $('#notificationContainer').append($notification);

                    // Attach click event handler to close button
                    $notification.find('.closeButton').click(function () {
                        $(this).parent('.notification').remove();
                        markNotificationAsRead(notification.id);
                    });
                });
            },
            error: function (xhr, status, error) {
                console.error('Error fetching notifications:', error);
            }
        });
    }

    function markNotificationAsRead(notificationId) {
        $.ajax({
            url: '/Tasks/MarkNotificationAsRead',
            type: 'POST',
            data: { id: notificationId },
            success: function () {
                console.log('Notification marked as read.');
            },
            error: function (xhr, status, error) {
                console.error('Error marking notification as read:', error);
            }
        })
    }

    function notificationSoundPlayed(notificationId) {
        $.ajax({
            url: '/Tasks/NotificationSoundPlayed',
            type: 'POST',
            data: { id: notificationId },
            success: function () {
                console.log('Notification sound marked as played.');
            },
            error: function (xhr, status, error) {
                console.error('Error marking the notification sound as played:', error);
            }
        })
    }
    function playNotificationSound() {
        var audio = document.getElementById('notificationSound');
        audio.play();


    }
    function playSilentAudio() {
        var audio = document.getElementById('silentAudio');
        audio.play();
    }
    playSilentAudio();
    // Schedule to fetch notifications every 30 seconds
    setInterval(fetchNotifications, 30000);


});