(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('User', User);

  User.$inject = ['dataservice', "$log", "$stateParams", '$state', '$uibModal'];
  function User(dataservice, $log, $stateParams, $state, $uibModal){
    var vm = this;


    vm.user = {};
    vm.roles = ["Standard", "Executive", "Super"];
    vm.editing = true;
    vm.groups = [];
    vm.goal = {};
    vm.goals = [];

    vm.save = save;
    vm.remove = remove;
    vm.onGroupSelect = onGroupSelect;
    vm.removeGroup = removeGroup;
    vm.showGoals = showGoals;

    activate();

    function activate() {
      getGroups().then(function () {
        if($stateParams.id !== "new")
          getUser($stateParams.id).then(function () {
            vm.groups = _.differenceBy(vm.groups,vm.user.groups, function (r) {
              return r.id;
            });
          });
        else {
          vm.editing = false;
          vm.user = {
            role: vm.roles[0],
            groups: []
          };
        }
      });
    }

    function getGroups() {
      return dataservice.userGroups
        .get()
        .then(function (data) {
          vm.groups = data;
          return vm.groups;
        });
    }

    function getUser(id) {
      return dataservice.users
        .get(id)
        .then(function (data) {
          vm.user = data;
          return vm.user;
        });
    }

    function save() {
      if(vm.editing){
        dataservice.users
          .update($stateParams.id, vm.user)
          .then(function () {
            $state.go("Users");
          });
      }
      else{
        dataservice.users
          .create(vm.user)
          .then(function () {
            $state.go("Users");
          });
      }
    }

    function remove() {
      dataservice.users
        .remove($stateParams.id)
        .then(function () {
          $state.go("Users");
        });
    }

    function removeGroup(group) {
      vm.user.groups = vm.user.groups.filter(function (g) { return g.id != group.id;});
      vm.groups.push(group);
    }

    function onGroupSelect(grp) {
      vm.user.groups.push(grp);
      vm.groups = vm.groups.filter(function (g) {return g.id != grp.id;});
      vm.group = "";
    }

    function showGoals() {
      $uibModal.open({
        controller: "Goals",
        controllerAs: "vm",
        size: "lg",
        templateUrl: "view/app/users/goals.html",
        resolve: {
          userId: function () { return $stateParams.id; },
          user: function () { return vm.user; }
        }
      });
    }
  }
})();
