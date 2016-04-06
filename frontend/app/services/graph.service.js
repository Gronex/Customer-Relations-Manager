(function() {
  'use strict';

  const monthFormat = "MMM YYYY";

  angular
    .module('CRM')
    .service('graph', graph );

  graph.$inject = ['dataservice', '$q'];

  /* @ngInject */
  function graph (dataservice, $q) {

    google.charts.load('current', {'packages':['corechart', 'table']});

    return {
      productionGraph: productionGraph,
      activityGraph: activityGraph,
      drawChart: drawChart,
      drawTable: drawTable
    };

    function drawChart(data, options, divId) {
      google.charts.setOnLoadCallback(function () {
        if(!divId) divId = 'chart_div';
        // Instantiate and draw our chart, passing in some options.
        var chart = new google.visualization.ComboChart(document.getElementById(divId));
        var table = new google.visualization.DataTable(data);
        chart.draw(table, options);
      });
    }

    function drawTable(data, options, divId) {
      google.charts.setOnLoadCallback(function () {
        if(!divId) divId = 'table_div';
        // Instantiate and draw our chart, passing in some options.
        var chart = new google.visualization.Table(document.getElementById(divId));
        var table = new google.visualization.DataTable(data);
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

    function handleProductionData(data, keys, filter){
      var result = [];
      data = convertToDate(data,keys);

      forRange(filter.from, filter.to, function(date){
        var row = [];
        row.push({v: date.toDate(), f: date.format(monthFormat)});
        for(var key of keys){
          var valueHolder = _.find(data[key], function(p){return p.period.isSame(date);});
          if(valueHolder)
            row.push({v: valueHolder.value});
          else row.push(undefined);
        }
        result.push({c: row});
      });
      return result;
    }

    function handleGoalData(data, keys, filter){
      data = convertToDate(data,keys);
      var res = [];
      var sums = {};

      for(var key in keys){
        sums[key] = 0;
      }

      forRange(filter.from, filter.to, function(date){
        var sum = 0;
        for(var key of keys){
          var d = _.find(data[key], function(g){return g.period.isSame(date);});
          if(d)
            sums[key] = d.value;
          if(sums[key])
            sum += sums[key];
        }
        res.push([{v: date.toDate(), f: date.format(monthFormat)},  {v: sum } ]);
      });
      return res;
    }

    function productionGraph(config) {
      var headers = [];
      var filter = {};
      var production = dataservice.graph
            .get({id: "production", query: config})
            .then(function(result){
              filter = {
                from: moment(result.from),
                to: moment(result.to)
              };
              return result.data;
            });

      var goals = dataservice.graph
            .get({id: "goal", query: config})
            .then(function(result){return result.data;});

      return $q.all({production: production, goals: goals})
        .then(function (result) {
          var data = [];
          var headers = [];

          headers.push({label: 'Month', type: 'date', format: "MMM"});

          var emails = _.union(Object.keys(result.goals), Object.keys(result.production));
          for (var email of emails) {
            var user = _.head(result.goals[email]|| result.production[email]).user;
            headers.push({label: user.firstName + " " + user.lastName, type: 'number'});
          }
          headers.push({label: 'Goal', type: 'number'});

          data = handleProductionData(result.production, emails, filter);
          var goalData = handleGoalData(result.goals, emails, filter);

          for(var i in data){
            data[i].c.push(goalData[i][1]);
          }
          var goalSeries = {};
          goalSeries[emails.length] = {type: 'line'};

          return {
            data: {
              cols: headers,
              rows: data
            },
            graphOptions: {
              title: 'Production',
              isStacked: true,
              legend: {position: 'top', maxLines: 3},
              hAxis: {title: "Time", format: "MMM yyyy"},
              vAxis: {title: "DKK"},
              seriesType: "steppedArea",
              series: goalSeries
            },
            tableOptions: {
              width: '100%',
              height: '100%'
            }
          };
        });
    }
    function activityGraph(){
      return dataservice.graph
        .get("activities")
        .then(function(result){
          var headers = [{label: "Category", type: "string"}, {label: "Count", type: "number"}];
          var data = _.map(result.data, function(a){ return {c: [{v: a.label}, {v: a.value}]}; });

          return {
            data: {
              cols: headers,
              rows: data
            },
            graphOptions: {
              title: 'Activity',
              legend: {position: 'top', maxLines: 3},
              hAxis: {title: "Amount"},
              vAxis: {title: "Category"},
              seriesType: "bars"
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
