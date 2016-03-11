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

    function drawChart(rows, cols, options, divId) {

      google.charts.setOnLoadCallback(function () {
        if(!divId) divId = 'chart_div';
        // Instantiate and draw our chart, passing in some options.
        var chart = new google.visualization.ComboChart(document.getElementById(divId));
        var table = new google.visualization.DataTable({
          cols: cols,
          rows: rows
        });
        chart.draw(table, options);
      });
    }

    function drawTable(rows, cols, options, divId) {
      google.charts.setOnLoadCallback(function () {
        if(!divId) divId = 'table_div';
        // Instantiate and draw our chart, passing in some options.
        var chart = new google.visualization.Table(document.getElementById(divId));
        var table = new google.visualization.DataTable({
          cols: cols,
          rows: rows
        });
        chart.draw(table, options);
      });
    }

    function convertToDate(data, keys){
      var res = {};
      for(var key of keys){
        res[key] = _.map(data[key], function(p){
          p.period = moment.utc(p.period);
          return p;
        });
      }
      return res;
    }

    function forRange(from, to, callback){
      var date = moment.utc(from);
      while(date < to){
        callback(date);
        date = date.clone();
        date.add(1, 'M');
      }
    }

    function handleProductionData(data, keys, startDate, endDate){
      var result = [];
      data = convertToDate(data,keys);

      forRange(startDate, endDate, function(date){
        var row = [];
        row.push({v: date.toDate()});
        for(var key of keys){
          var valueHolder = _.find(data[key], function(p){return p.period.isSame(date);});
          if(valueHolder)
            row.push({v: valueHolder.value});
          else row.push(undefined);
        }
        result.push({c: row});
      });
      console.log(result);
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
          var d = _.find(data[key], function(g){return g.period.isSame(date);});
          if(d)
            sums[key] = d.value;
          if(sums[key])
            sum += sums[key];
        }
        res.push([{v: date.toDate()},  {v: sum } ]);
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
          var data = [];
          var headers = [];

          headers.push({label: 'Month', type: 'date'});

          var emails = Object.keys(result.goals);
          for (var email of emails) {
            var user = _.head(result.goals[email]).user;
            headers.push({label: user.firstName + " " + user.lastName, type: 'number'});
          }
          headers.push({label: 'Goal', type: 'number'});

          data = handleProductionData(result.production, emails, config.startDate, config.endDate);
          var goalData = handleGoalData(result.goals, emails, config.startDate, config.endDate);

          for(var i in data){
            data[i].c.push(goalData[i][1]);
          }
          var goalSeries = {};
          goalSeries[emails.length] = {type: 'line'};

          return {
            cols: headers,
            rows: data,
            graphOptions: {
              title: 'Production',
              isStacked: true,
              legend: {position: 'top', maxLines: 3},
              hAxis: {title: "Time"},
              vAxis: {title: "DKK"},
              seriesType: "bars",
              series: goalSeries
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
