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
        });
    }
  }
})();
