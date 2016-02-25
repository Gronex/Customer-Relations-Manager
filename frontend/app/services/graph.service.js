(function() {
  'use strict';

  angular
    .module('CRM')
    .service('graph', graph );

  graph.$inject = ['$http'];

  /* @ngInject */
  function graph ($http) {
    return {
      productionGraph: productionGraph
    };

    function productionGraph() {
      return $http.get("api/graph/production")
        .then(function (result) {
          return result.data;
        });
    }
  }
})();
