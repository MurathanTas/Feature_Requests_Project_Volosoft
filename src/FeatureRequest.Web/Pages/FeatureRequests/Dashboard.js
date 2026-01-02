$(function () {
  var $chartContainer = $('#chartDataContainer');

  if (!$chartContainer.length) {
    console.warn('Chart data container not found');
    return;
  }

  var categoryLabels = JSON.parse($chartContainer.attr('data-category-labels') || '[]');
  var categoryData = JSON.parse($chartContainer.attr('data-category-data') || '[]');
  var categoryVotes = JSON.parse($chartContainer.attr('data-category-votes') || '[]');
  var statusLabels = JSON.parse($chartContainer.attr('data-status-labels') || '[]');
  var statusData = JSON.parse($chartContainer.attr('data-status-data') || '[]');

  var colors = [
    'rgba(54, 162, 235, 0.8)',
    'rgba(255, 99, 132, 0.8)',
    'rgba(255, 206, 86, 0.8)',
    'rgba(75, 192, 192, 0.8)',
    'rgba(153, 102, 255, 0.8)',
    'rgba(255, 159, 64, 0.8)'
  ];

  var categoryChartEl = document.getElementById('categoryChart');
  if (categoryChartEl) {
    new Chart(categoryChartEl, {
      type: 'bar',
      data: {
        labels: categoryLabels,
        datasets: [{
          label: 'İstek Sayısı',
          data: categoryData,
          backgroundColor: colors,
          borderWidth: 1
        }]
      },
      options: {
        responsive: true,
        plugins: { legend: { display: false } },
        scales: { y: { beginAtZero: true } }
      }
    });
  }

  var statusChartEl = document.getElementById('statusChart');
  if (statusChartEl) {
    new Chart(statusChartEl, {
      type: 'doughnut',
      data: {
        labels: statusLabels,
        datasets: [{ data: statusData, backgroundColor: colors }]
      },
      options: {
        responsive: true,
        plugins: { legend: { position: 'right' } }
      }
    });
  }

  var categoryVotesChartEl = document.getElementById('categoryVotesChart');
  if (categoryVotesChartEl) {
    new Chart(categoryVotesChartEl, {
      type: 'bar',
      data: {
        labels: categoryLabels,
        datasets: [{
          label: 'Toplam Oy',
          data: categoryVotes,
          backgroundColor: 'rgba(40, 167, 69, 0.8)',
          borderWidth: 1
        }]
      },
      options: {
        responsive: true,
        plugins: { legend: { display: false } },
        scales: { y: { beginAtZero: true } }
      }
    });
  }
});
