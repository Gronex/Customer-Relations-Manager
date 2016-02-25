(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('Home', Home);

  Home.$inject = ['graph'];

  /* @ngInject */
  function Home(graph) {
    var vm = this;

    activate();

    function activate() {
      getProductionGraph();
    }

    function getProductionGraph(){
      graph.productionGraph()
        .then(function (data) {
          console.log(data);
          vm.productionData = data.set;

          vm.options = {
            series: [
              {
                axis: "y",
                dataset: "test",
                key: "value",
                label: "An area series",
                color: "#1f77b4",
                type: ['line', 'dot', 'area'],
                id: 'mySeries0'
              }
            ],
            axes: {x: {
              type: 'date', // or 'date', or 'log'
              key: "label"
            }}
          };
        });
    }
  }
})();
