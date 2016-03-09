(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('Home', Home);

  Home.$inject = ['graph', '$state', '$stateParams'];

  /* @ngInject */
  function Home(graph, $state, $stateParams) {
    var vm = this;
    var today = new Date();

    vm.filter = {};

    vm.getProductionGraph = getProductionGraph;


    activate();

    function activate() {
      setupFilter();
      getProductionGraph();
    }

    function getProductionGraph(){

      $state.go("Home", vm.filter, {notify: false});

      var config = {
        startDate: new Date(Date.UTC(vm.filter.fromYear, vm.filter.fromMonth - 1)),
        endDate: new Date(Date.UTC(vm.filter.toYear, vm.filter.toMonth - 1))
      };
      graph.productionGraph(config)
        .then(function (result) {
          graph.drawChart(result.data, result.graphOptions);
          graph.drawTable(result.data, result.tableOptions);
        });
    }

    function setupFilter() {
      vm.filter = _.merge({
        fromMonth: 1,
        fromYear: today.getFullYear(),
        toMonth: 1,
        toYear: today.getFullYear() + 1
      },$stateParams);
    }
  }
})();
