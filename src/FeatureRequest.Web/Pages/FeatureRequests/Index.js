(function () {
  var service = featureRequest.featureRequests.featureRequest;

  function reorderItems() {
    var $container = $('#RequestListContainer');
    var $items = $container.children('.list-group-item');

    $items.sort(function (a, b) {
      var voteA = parseInt($(a).attr('data-votes')) || 0;
      var voteB = parseInt($(b).attr('data-votes')) || 0;
      return voteB - voteA;
    });

    $items.detach().appendTo($container);
  }

  $('.upvote-btn').click(function (e) {
    e.preventDefault();
    var $button = $(this);
    var id = $button.attr('data-id');
    var isVoted = $button.attr('data-voted') === 'true';
    var $countSpan = $button.find('.vote-count');
    var $icon = $button.find('i');

    $button.prop('disabled', true);

    if (!isVoted) {
      service.upvote(id).then(function () {
        var currentCount = parseInt($countSpan.text());
        $countSpan.text(currentCount + 1);

        $button.removeClass('btn-outline-success').addClass('btn-success');
        $button.attr('data-voted', 'true');
        $icon.removeClass('fa-arrow-up').addClass('fa-check');

        abp.notify.success('Oy verildi! 🚀');
        $button.prop('disabled', false);
        $button.closest('.list-group-item').attr('data-votes', currentCount + 1);
        reorderItems();
      }).catch(function (err) {
        $button.prop('disabled', false);
        abp.notify.error('Bir hata oluştu.');
      });
    }
    else {
      service.downvote(id).then(function () {
        var currentCount = parseInt($countSpan.text());
        $countSpan.text(currentCount - 1);

        $button.removeClass('btn-success').addClass('btn-outline-success');
        $button.attr('data-voted', 'false');
        $icon.removeClass('fa-check').addClass('fa-arrow-up');

        abp.notify.info('Oy geri çekildi.');
        $button.prop('disabled', false);
        $button.closest('.list-group-item').attr('data-votes', currentCount - 1);
        reorderItems();
      }).catch(function (err) {
        $button.prop('disabled', false);
        abp.notify.error('Bir hata oluştu.');
      });
    }
  });

  $('#categoryFilter').change(function () {
    var selectedCategory = $(this).val();
    var pageSize = $('#pageSizeFilter').val();
    var url = '/FeatureRequests?PageSize=' + pageSize;
    if (selectedCategory) {
      url += '&SelectedCategory=' + selectedCategory;
    }
    window.location.href = url;
  });

  $('#pageSizeFilter').change(function () {
    var selectedCategory = $('#categoryFilter').val();
    var pageSize = $(this).val();
    var url = '/FeatureRequests?PageSize=' + pageSize;
    if (selectedCategory) {
      url += '&SelectedCategory=' + selectedCategory;
    }
    window.location.href = url;
  });
})();
