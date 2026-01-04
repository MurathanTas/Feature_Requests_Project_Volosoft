(function () {
  var service = featureRequest.featureRequests.featureRequest;

  $('#statusFilter, #categoryFilter').change(function () {
    var status = $('#statusFilter').val();
    var category = $('#categoryFilter').val();
    var pageSize = $('#pageSizeFilter').val();
    var url = '/Admin/FeatureRequests?PageSize=' + pageSize;

    if (status) url += '&SelectedStatus=' + status;
    if (category) url += '&SelectedCategory=' + category;

    window.location.href = url;
  });

  $('#pageSizeFilter').change(function () {
    var status = $('#statusFilter').val();
    var category = $('#categoryFilter').val();
    var pageSize = $(this).val();
    var url = '/Admin/FeatureRequests?PageSize=' + pageSize;

    if (status) url += '&SelectedStatus=' + status;
    if (category) url += '&SelectedCategory=' + category;

    window.location.href = url;
  });

  $('.status-change-btn').click(function (e) {
    e.preventDefault();
    var $link = $(this);
    var id = $link.data('id');
    var status = $link.data('status');
    var statusName = $link.data('status-name');

    var l = abp.localization.getResource('FeatureRequest');

    abp.message.confirm(
      l('AdminPage:ConfirmStatusChange', statusName),
      l('AdminPage:ConfirmStatusChangeTitle')
    ).then(function (confirmed) {
      if (confirmed) {
        service.updateStatus(id, status).then(function () {
          abp.notify.success(l('AdminPage:StatusUpdateSuccess'));

          // Sayfa yenilemeden sadece o satırı güncelle
          var $row = $link.closest('tr');
          var $statusBadge = $row.find('td:eq(2) .badge');

          // Badge sınıfını ve metnini güncelle
          $statusBadge.removeClass('bg-secondary bg-warning bg-success bg-primary bg-dark bg-danger text-dark');
          $statusBadge.addClass(getStatusBadgeClass(status));
          $statusBadge.text(statusName);

          // Dropdown'daki active sınıfını güncelle
          $row.find('.status-change-btn').removeClass('active');
          $link.addClass('active');

        }).catch(function (err) {
          console.error(err);
          abp.notify.error(l('AdminPage:StatusUpdateError'));
        });
      }
    });
  });

  // Status badge sınıfını döndüren yardımcı fonksiyon
  function getStatusBadgeClass(status) {
    switch (status) {
      case 0: return 'bg-secondary'; // Draft
      case 1: return 'bg-warning text-dark'; // Pending
      case 2: return 'bg-success'; // Approved
      case 3: return 'bg-primary'; // Planned
      case 4: return 'bg-dark'; // Completed
      case 5: return 'bg-danger'; // Rejected
      default: return 'bg-secondary';
    }
  }

  $('.delete-request-btn').click(function (e) {
    e.preventDefault();
    var $button = $(this);
    var id = $button.data('id');
    var title = $button.data('title');

    var l = abp.localization.getResource('FeatureRequest');

    abp.message.confirm(
      l('AdminPage:ConfirmDelete', title),
      l('AdminPage:ConfirmDeleteTitle')
    ).then(function (confirmed) {
      if (confirmed) {
        service.delete(id).then(function () {
          abp.notify.success(l('AdminPage:DeleteSuccess'));
          // Silinen satırı DOM'dan kaldır
          $button.closest('tr').fadeOut(300, function () {
            $(this).remove();
          });
        }).catch(function (err) {
          console.error(err);
          abp.notify.error(l('AdminPage:DeleteError'));
        });
      }
    });
  });
})();
