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

    abp.message.confirm(
      'Durumu "' + statusName + '" olarak değiştirmek istediğinize emin misiniz?',
      'Durum Değiştir'
    ).then(function (confirmed) {
      if (confirmed) {
        service.updateStatus(id, status).then(function () {
          abp.notify.success('Durum başarıyla güncellendi!');
          location.reload();
        }).catch(function (err) {
          console.error(err);
          abp.notify.error('Bir hata oluştu.');
        });
      }
    });
  });
})();
