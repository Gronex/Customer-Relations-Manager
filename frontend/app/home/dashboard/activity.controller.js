(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('ActivityGraph', Controller);

  Controller.$inject = ['$scope', 'graph', '$state', 'dataservice', '$uibModal', 'authorization', 'filter', 'stateCom', 'dateFormat'];

  /* @ngInject */
  function Controller($scope, graph, $state, dataservice, $modal, auth, filter, stateCom, dateFormat) {
    var vm = this;

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

      stateCom.setupFunction("activity", getActivityGraph);

      getActivityGraph();
      getFilters();
      getFilterOptions();
    }

    function getActivityGraph(){
      var stringFilter = {
        fromDate: moment(vm.filter.fromDate).format(dateFormat),
        toDate: moment(vm.filter.toDate).format(dateFormat)
      };

      $state.go("Dashboard", stringFilter, {notify: false});
      var config = {
        startDate: vm.filter.fromDate,
        endDate: vm.filter.toDate,
        userGroups: _.map(vm.advancedFilter.userGroups, "id"),
        users: _.map(vm.advancedFilter.users, "email")
      };

      graph.activityGraph(config)
        .then(function (result) {
          graph.drawChart(result.data, result.graphOptions, "activity_chart");
          graph.drawTable(result.data, result.tableOptions, "activity_table");
        });
    }

    function getFilters(){
      return dataservice.activityGraphFilters
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
      return dataservice.activityGraphFilters
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
      return [userGroupTask, userTask];
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
                dataservice.activityGraphFilters
                  .create(vm.advancedFilter)
                  .then(function(result){
                    var data = {id: result.location, name: result.data.name};
                    vm.savedFilters.push(data);
                    vm.advancedFilter = result.data;
                    vm.savedFilter = data;
                    var user =  auth.getUser();
                    vm.owner = user.roles.includes("Super") || user.email === result.ownerEmail;
                  });
              };
            },
            cancel: function(){}
          }
        });
      } else {
        dataservice.activityGraphFilters
          .update(vm.savedFilter.id, vm.advancedFilter)
          .then(function(f){
            vm.advancedFilterChanged = false;
            vm.advancedFilter = f;
          });
      }
    }

    function remove(){
      dataservice.activityGraphFilters
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
