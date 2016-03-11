(function() {
  'use strict';

  const dateFormatString = 'YYYY-MM-DD';

  angular
    .module('CRM')
    .controller('Home', Home);

  Home.$inject = ['graph', '$state', '$stateParams'];

  /* @ngInject */
  function Home(graph, $state, $stateParams) {
    var vm = this;
    vm.filter = {};
    var filter = {};

    vm.getProductionGraph = getProductionGraph;

    activate();

    function activate() {
      setupFilter();
      getProductionGraph();
    }

    function getProductionGraph(){
      filter.fromDate = moment(filter.fromDate).format(dateFormatString);
      filter.toDate = moment(filter.toDate).format(dateFormatString);

      $state.go("Home", filter, {notify: false});
      var config = {
        startDate: vm.filter.fromDate,
        endDate: vm.filter.toDate
      };
      graph.productionGraph(config)
        .then(function (result) {
          graph.drawChart(result.data, result.graphOptions);
          graph.drawTable(result.data, result.tableOptions);
        });
    }

    function setupFilter() {
      filter = _.merge({
        fromDate: moment.utc().startOf('year').format(dateFormatString),
        toDate: moment.utc().add(1, 'year').startOf('year').format(dateFormatString)
      },$stateParams);

      vm.filter = {
        fromDate: moment.utc(filter.fromDate).toDate(),
        toDate: moment.utc(filter.toDate).toDate()
      };
    }
  }
})();
