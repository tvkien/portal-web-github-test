var notification = {
  getUnreadNotification: function () {
      return $.ajax({
        type: 'POST',
        url: URLs.loadUnreadMessage
      });
  },
  getListNotification: function (currentIndex) {
      return $.ajax({
          type: 'POST',
        url: URLs.getNotification,
          data: {
              'currentIndex': currentIndex
          }
      });
  }
};

var notificationVM = new Vue({
    el: '#notification',
    data: {
        isShowLoading: false,
        isShowDropdown: false,
        isBusy: false,
        isLoadMore: true,
        countUnread: 0,
        countIndex: 0,
        listNotification: [],
        remainUnread: 0
    },
    ready: function () {
        var self = this;

        notification.getUnreadNotification().done(function (data) {
            self.countUnread = data;
            self.remainUnread = data;
        });
    },
    computed: {
        isAllowScroll: function () {
            return this.isShowDropdown;
        }
    },
    methods: {
        toggleDropdown: function () {
            var self = this;

            self.isShowDropdown = !self.isShowDropdown;
            self.countUnread = 0;

            // Remove unread notification when close dropdown
            if (!self.isShowDropdown) {
                self.removeIsUnreadNotification();
            }

            // Get list notification message
            if (self.isShowDropdown && !self.listNotification.length) {
                self.isShowLoading = true;

                notification.getListNotification(self.countIndex).done(function (data) {
                    self.updateIsUnreadNotification(self.remainUnread, data);
                    self.countIndex = self.countIndex + data.length;
                    self.isShowLoading = false;
                    self.listNotification = data;
                });
            }
        },
        loadMoreListNotification: function () {
            var self = this;

            // Load more list notification when scrolling
            if (self.isLoadMore && self.isAllowScroll) {
                self.isBusy = true;
                self.isShowLoading = true;

                setTimeout(function () {
                    notification.getListNotification(self.countIndex).done(function (data) {
                        self.isBusy = false;
                        self.isShowLoading = false;

                        if (data.length) {
                            self.updateIsUnreadNotification(self.remainUnread, data);
                            self.countIndex = self.countIndex + data.length;
                            self.listNotification = self.listNotification.concat(data);
                        } else {
                            self.isLoadMore = false;
                        }
                    });
                }, 1000);
            }
        },
        updateIsUnreadNotification: function (remainUnread, data) {
            var self = this;

            if (remainUnread > 0) {
                var i = 0;
                var lenData = data.length;

                if (remainUnread > data.length) {
                    for (i = 0; i < lenData; i++) {
                        data[i].IsUnread = true;
                    }

                    self.remainUnread = remainUnread - lenData;
                } else {
                    for (i = 0; i < remainUnread; i++) {
                        data[i].IsUnread = true;
                    }

                    self.remainUnread = 0;
                }
            }
        },
        removeIsUnreadNotification: function () {
            var self = this;
            self.isShowDropdown = false;

            if (self.listNotification.length) {
                self.remainUnread = 0;

                for (var i = 0, len = self.listNotification.length; i < len; i++) {
                    self.listNotification[i].IsUnread = false;
                }
            }
        },
        mouseoverNotification: function (item) {
            item.IsUnread = false;
        }
    }
});
