(function () {
  var featureRequestService = featureRequest.featureRequests.featureRequest;
  var commentService = featureRequest.featureRequests.featureRequestComment;

  $('#voteButton').click(function () {
    var $button = $(this);
    var id = $button.attr('data-id');
    var isVoted = $button.attr('data-voted') === 'true';

    var $countSpan = $('#VoteCount');
    var $textSpan = $('#voteText');
    var $helpText = $('#voteHelpText');
    var $icon = $button.find('i');

    $button.prop('disabled', true);

    if (!isVoted) {
      featureRequestService.upvote(id).then(function () {
        var currentCount = parseInt($countSpan.text());
        $countSpan.text(currentCount + 1);

        $button.removeClass('btn-outline-success').addClass('btn-success');
        $button.attr('data-voted', 'true');
        $icon.removeClass('fa-arrow-up').addClass('fa-check');
        $textSpan.text("Oy Verildi");
        $helpText.text("Oyunuzu geri çekmek için tıklayın");

        abp.notify.success('Oyunuz başarıyla kaydedildi!');
        $button.prop('disabled', false);
      }).catch(handleError);
    }
    else {
      featureRequestService.downvote(id).then(function () {
        var currentCount = parseInt($countSpan.text());
        $countSpan.text(currentCount - 1);

        $button.removeClass('btn-success').addClass('btn-outline-success');
        $button.attr('data-voted', 'false');
        $icon.removeClass('fa-check').addClass('fa-arrow-up');
        $textSpan.text("Oy Ver");
        $helpText.text("Desteklemek için tıklayın");

        abp.notify.info('Oyunuz geri çekildi.');
        $button.prop('disabled', false);
      }).catch(handleError);
    }

    function handleError(err) {
      $button.prop('disabled', false);
      console.error(err);
      abp.notify.error('Bir hata oluştu.');
    }
  });

  $('#submitCommentBtn').click(function () {
    var $button = $(this);
    var $textarea = $('#commentText');
    var $error = $('#commentError');
    var commentText = $textarea.val().trim();
    var featureRequestId = $('#featureRequestId').val();

    $error.text('');
    if (!commentText) {
      $error.text('Yorum boş olamaz.');
      return;
    }

    $button.prop('disabled', true);
    $button.html('<i class="fa fa-spinner fa-spin"></i> Gönderiliyor...');

    commentService.create({
      featureRequestId: featureRequestId,
      commentText: commentText
    }).then(function (result) {
      var now = new Date();
      var formattedDate = now.toLocaleDateString('tr-TR') + ' ' + now.toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' });

      var newCommentHtml = `
                <div class="list-group-item list-group-item-action flex-column align-items-start mb-2 border rounded bg-white" style="animation: fadeIn 0.3s;">
                    <div class="d-flex w-100 justify-content-between">
                        <h6 class="mb-1 fw-bold text-primary">
                            <i class="fa fa-user-circle"></i> ${abp.currentUser.userName}
                        </h6>
                        <small class="text-muted">${formattedDate}</small>
                    </div>
                    <p class="mb-1 text-dark mt-2">${commentText}</p>
                </div>
            `;

      $('#commentsContainer').prepend(newCommentHtml);
      $('#noCommentsText').hide();

      var currentCount = parseInt($('#commentCount').text()) || 0;
      $('#commentCount').text(currentCount + 1);

      $textarea.val('');

      abp.notify.success('Yorumunuz eklendi! ');

      $button.prop('disabled', false);
      $button.html('<i class="fa fa-paper-plane"></i> Gönder');
    }).catch(function (err) {
      console.error(err);
      $error.text('Yorum eklenirken bir hata oluştu.');
      $button.prop('disabled', false);
      $button.html('<i class="fa fa-paper-plane"></i> Gönder');
    });
  });
})();
