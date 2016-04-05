(function() {
  'use strict';


  angular
    .module('CRM')
    .controller('Dashboard', Dashboard);

  Dashboard.$inject = ['$state', 'filter', 'stateCom'];

  /* @ngInject */
  function Dashboard($state, filter, stateCom) {
    var vm = this;

    vm.filterFunc = stateCom.invoke;

    activate();

    function activate() {
      vm.filter = filter;
    }
  }
})();
