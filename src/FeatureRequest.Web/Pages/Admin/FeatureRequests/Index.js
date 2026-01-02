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
          location.reload();
        }).catch(function (err) {
          console.error(err);
          abp.notify.error(l('AdminPage:StatusUpdateError'));
        });
      }
    });
  });

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
