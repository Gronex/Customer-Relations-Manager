(function() {
  'use strict';


  angular
    .module('CRM')
    .controller('Dashboard', Dashboard);

  Dashboard.$inject = ['$state', 'filter', 'state', 'stateCom', 'dateFormat'];

  /* @ngInject */
  function Dashboard($state, filter, state, stateCom, dateFormat) {
    var vm = this;

    vm.filterFunc = stateCom.invoke;
    vm.tabChange = tabChange;

    activate();

    function activate() {
      vm.filter = filter;
      vm.tabs = [
        {
          heading: "Production",
          view: "production",
          active: state == "production"
        },
        {
          heading: "Activity",
          view: "activity",
          active: state == "activity"
        }
      ];
    }

    function tabChange(tab){
      vm.tab = tab.view;
      for(var t in vm.tabs){
        vm.tabs[t].active = false;
        tab.active = true;
      }
      updateUrl(tab.view);
      stateCom.resend(tab.view);
    }

    function updateUrl(state){
      var query = {
        fromDate: moment(vm.filter.fromDate).format(dateFormat),
        toDate: moment(vm.filter.toDate).format(dateFormat),
        state: state
      };
      $state.go("Dashboard", query, {notify: false});
    }
  }
})();
