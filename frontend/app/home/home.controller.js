(function() {
  'use strict';

  const dateFormatString = 'YYYY-MM-DD';

  angular
    .module('CRM')
    .controller('Home', Home);

  Home.$inject = ['$scope', 'graph', '$state', '$stateParams', 'dataservice'];

  /* @ngInject */
  function Home($scope, graph, $state, $stateParams, dataservice) {
    var vm = this;
    vm.filter = {};
    vm.savedFilter = {name: "select filter"};
    vm.savedFilters = [];
    vm.advancedFilter = {};
    var filter = {};

    vm.getProductionGraph = getProductionGraph;
    vm.getFilter = getFilter;
    vm.save = save;

    activate();

    function activate() {
      setupFilter();
      getProductionGraph();
      getFilters();
      getFilterOptions();
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

    function getFilters(){
      return dataservice.productionGraphFilters
        .get()
        .then(function(filters){
          vm.savedFilters = filters;
          vm.savedFilters.splice(0,0,{name: "select filter"});
        });
    }

    function getFilter(){
      return dataservice.productionGraphFilters
        .get(vm.savedFilter.id)
        .then(function(f){
          vm.advancedFilter = f;
          vm.advancedFilterChanged = false;
        });
    }

    function getFilterOptions(){
      vm.filterOptions = {};
      var departmentTask = dataservice.departments
            .get()
            .then(function(departments){
              vm.filterOptions.departments = departments;
            });
      var stageTask = dataservice.stages
            .get()
            .then(function(stages){
              vm.filterOptions.stages = stages;
            });
      var userGroupTask = dataservice.userGroups
            .get()
            .then(function(userGroups){
              vm.filterOptions.userGroups = userGroups;
            });
      var userTask = dataservice.users
            .get()
            .then(function(users){
              vm.filterOptions.users = users.data;
            });
      var categoryTask = dataservice.opportunityCategories
            .get()
            .then(function(categories){
              vm.filterOptions.categories = categories;
            });
      return [departmentTask, stageTask, userGroupTask, userTask, categoryTask];
    }

    $scope.$watch('vm.advancedFilter', function(newValue, oldValue){
      if(newValue.name === oldValue.name){
        vm.advancedFilterChanged = true;
      }
    },true);

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

    function save(state){
      if(state === "new"){
        console.log("save...");
        console.log(vm.advancedFilter);
      } else {
        console.log("save");
        console.log(vm.advancedFilter);
      }
    }
  }
})();
