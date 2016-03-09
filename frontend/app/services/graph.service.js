(function() {
  'use strict';

  angular
    .module('CRM')
    .service('graph', graph );

  graph.$inject = ['dataservice', '$q'];

  /* @ngInject */
  function graph (dataservice, $q) {

    google.charts.load('current', {'packages':['corechart', 'table']});

    return {
      productionGraph: productionGraph,
      drawChart: drawChart,
      drawTable: drawTable
    };

    function drawChart(data, options, divId) {

      google.charts.setOnLoadCallback(function () {
        if(!divId) divId = 'chart_div';
        // Instantiate and draw our chart, passing in some options.
        var chart = new google.visualization.ComboChart(document.getElementById(divId));
        chart.draw(data, options);
      });
    }

    function drawTable(data, options, divId) {
      google.charts.setOnLoadCallback(function () {
        if(!divId) divId = 'table_div';
        // Instantiate and draw our chart, passing in some options.
        var chart = new google.visualization.Table(document.getElementById(divId));
        chart.draw(data, options);
      });
    }

    function convertToDate(data, keys){
      var res = {};
      for(var key of keys){
        res[key] = _.map(data[key], function(p){
          p.period = new Date(p.period);
          return p;
        });
      }
      return res;
    }

    function forRange(from, to, callback){
      var date = from;
      while(date < to){
        callback(date);
        date = new Date(date);
        date.setMonth(date.getMonth() + 1);
      }
    }

    function handleProductionData(data, keys, startDate, endDate){
      var result = [];
      data = convertToDate(data,keys);

      forRange(startDate, endDate, function(date){
        var row = [];
        row.push(date);
        for(var key of keys){
          var valueHolder = _.find(data[key], function(p){return p.period.getTime() === date.getTime();});
          if(valueHolder)
            row.push(valueHolder.value);
          else row.push(0);
        }
        result.push(row);
      });
      return result;
    }

    function handleGoalData(data, keys, startDate, endDate){
      data = convertToDate(data,keys);
      var res = [];
      var sums = {};

      for(var key in keys){
        sums[key] = 0;
      }

      forRange(startDate, endDate, function(date){
        var sum = 0;
        for(var key of keys){
          var d = _.find(data[key], function(g){return g.period.getTime() === date.getTime();});
          if(d)
            sums[key] = d.value;
          if(sums[key])
            sum += sums[key];
        }
        res.push([new Date(date), sum]);
      });
      return res;
    }

    function productionGraph(config) {
      var headers = [];
      var production = dataservice.graph
            .get({id: "production", query: config});

      var goals = dataservice.graph
            .get({id: "goal", query: config});

      return $q.all({production: production, goals: goals})
        .then(function (result) {

          var data = new google.visualization.DataTable();
          data.addColumn('date', 'Month');

          var emails = Object.keys(result.goals);
          for (var email of emails) {
            var user = _.head(result.goals[email]).user;
            data.addColumn('number', user.firstName + " " + user.lastName);
          }
          data.addColumn('number', 'Goal');

          var prodData = handleProductionData(result.production, emails, config.startDate, config.endDate);
          var goalData = handleGoalData(result.goals, emails, config.startDate, config.endDate);
          console.log(goalData);

          for(var i in prodData){
            prodData[i].push(goalData[i][1]);
          }
          data.addRows(prodData);
          var goalSeries = {};
          goalSeries[emails.length] = {type: 'line'};

          return {
            data: data,
            graphOptions: {
              title: 'Production',
              isStacked: true,
              legend: {position: 'top', maxLines: 3},
              hAxis: {title: "Time"},
              vAxis: {title: "DKK"},
              seriesType: "area",
              series: goalSeries,
              crosshair: { trigger: 'both' }
            },
            tableOptions: {
              width: '100%',
              height: '100%'
            }
          };
        });
    }
  }
})();
