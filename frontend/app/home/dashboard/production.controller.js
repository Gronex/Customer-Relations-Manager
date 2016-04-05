(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('ProductionGraph', Controller);

  Controller.$inject = ['$scope', 'graph', '$state', 'dataservice', '$uibModal', 'authorization', 'filter', 'stateCom', 'dateFormat'];

  /* @ngInject */
  function Controller($scope, graph, $state, dataservice, $modal, auth, filter, stateCom, dateFormat) {
    var vm = this;

    vm.getProductionGraph = getProductionGraph;
    vm.getFilter = getFilter;
    vm.save = save;
    vm.resetFilter = resetFilter;
    vm.delete = remove;

    activate();

    function activate() {
      vm.savedFilter = {name: "select filter"};
      vm.savedFilters = [];
      vm.advancedFilter = { private: true};
      vm.owner = true;
      vm.filter = filter;

      stateCom.setupFunction(getProductionGraph);

      getProductionGraph();
      getFilters();
      getFilterOptions();
    }

    function getProductionGraph(){
      var stringFilter = {
        fromDate: moment(vm.filter.fromDate).format(dateFormat),
        toDate: moment(vm.filter.toDate).format(dateFormat)
      };

      $state.go("Dashboard.production", stringFilter, {notify: false});
      var config = {
        startDate: vm.filter.fromDate,
        endDate: vm.filter.toDate,
        departments: _.map(vm.advancedFilter.departments, "id"),
        stages: _.map(vm.advancedFilter.stages, "id"),
        userGroups: _.map(vm.advancedFilter.userGroups, "id"),
        users: _.map(vm.advancedFilter.users, "email"),
        categories: _.map(vm.advancedFilter.categories, "id"),
        weighted: vm.advancedFilter.weighted
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
      if(!vm.savedFilter.id){
        vm.advancedFilter = {};
        vm.advancedFilterChanged = false;
        vm.owner = true;
        return true;
      }
      return dataservice.productionGraphFilters
        .get(vm.savedFilter.id)
        .then(function(f){
          vm.advancedFilter = f;
          vm.advancedFilterChanged = false;
          var user =  auth.getUser();
          vm.owner = user.roles.includes("Super") || user.email === f.ownerEmail;
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

    function save(state){
      if(state === "new"){
        $modal.open({
          templateUrl: "view/app/home/save-modal.html",
          controllerAs: 'vm',
          controller: 'SaveModal',
          resolve: {
            save: function(){
              return function(name){
                vm.advancedFilter.name = name;
                dataservice.productionGraphFilters
                  .create(vm.advancedFilter)
                  .then(function(result){
                    var data = {id: result.location, name: result.data.name};
                    vm.savedFilters.push(data);
                    vm.advancedFilter = result.data;
                    vm.savedFilter = data;
                    var user = auth.user;
                    vm.owner = user.roles.includes("Super") || user.email === result.ownerEmail;
                  });
              };
            },
            cancel: function(){}
          }
        });
      } else {
        dataservice.productionGraphFilters
          .update(vm.savedFilter.id, vm.advancedFilter)
          .then(function(f){
            vm.advancedFilterChanged = false;
            vm.advancedFilter = f;
          });
      }
    }

    function remove(){
      dataservice.productionGraphFilters
        .remove(vm.savedFilter.id)
        .then(function(){
          _.remove(vm.savedFilters, function(f){return f.id === vm.savedFilter.id;});
          resetFilter();
        });
    }

    function resetFilter(){
      vm.savedFilter = {};
      vm.advancedFilter = {};
    }
  }
})();
