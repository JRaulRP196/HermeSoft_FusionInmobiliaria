
// Stacked Columns Charts
var options = {
  series: [{
    name: 'PAGOS AL DÍA',
    data: [44, 55, 41]
  }, {
    name: 'PAGOS PENDIENTES',
    data: [13, 23, 20]
  }, {
    name: 'PAGOS ATRASADOS',
    data: [11, 17, 15]
  }],
  chart: {
    type: 'bar',
    height: 350,
    stacked: true,
    toolbar: {
      show: false
    },
    zoom: {
      enabled: true
    },
    toolbar: {
      show: false,
    }
  },
  responsive: [{
    breakpoint: 480,
    options: {
      legend: {
        position: 'bottom',
        offsetX: -10,
        offsetY: 0
      }
    }
  }],
  plotOptions: {
    bar: {
      horizontal: false,
      borderRadius: 10
    },
  },
  xaxis: {
    type: 'category',
    categories: ['Condominio A', 'Condominio B', 'Condominio C'
    ],
  },
  legend: {
    position: 'right',
    offsetY: 40
  },
  fill: {
    opacity: 1
  },
  colors: ['#086070','#ed5e49','#2651e9'],
};

document.addEventListener("DOMContentLoaded", function () {
    var chart = new ApexCharts(document.querySelector("#column_stacked"), options);
    chart.render();
});
