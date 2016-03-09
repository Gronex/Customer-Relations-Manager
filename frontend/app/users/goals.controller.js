(function() {
  'use strict';

  angular
    .module('CRM')
    .controller('Goals', Goals);

  Goals.$inject = ['dataservice', '$uibModalInstance', 'userId', 'user'];

  /* @ngInject */
  function Goals(dataservice, $uibModalInstance, userId, user) {
    var vm = this;

    vm.user = user;

    vm.addGoal = addGoal;
    vm.removeGoal = removeGoal;
    vm.close = close;

    activate();

    function activate() {
      resetGoal();
      getUserGoals(userId);
    }


    function getUserGoals(id) {
      dataservice.goals.get({userId: id})
        .then(function (data) {
          vm.goals = _.orderBy(data, ['year', 'month']);
          return vm.goals;
        });
    }

    function addGoal() {
      vm.goal.errMsg = undefined;
      dataservice.goals.create(vm.goal, {userId: userId}).
        then(function (data) {
          vm.goals.push(data);
          vm.goals = _.orderBy(vm.goals, ['year', 'month']);
        }, function (err) {
          if(err.status === 409)
            vm.goal.errMsg = "For a single user there can only be one goal per month";
        });
      resetGoal();
    }

    function removeGoal(goal) {
      dataservice.goals
        .remove(goal.id, {userId: userId})
        .then(function () {
          vm.goals = vm.goals.filter(function (g) { return g.id != goal.id;});
        });
    }

    function resetGoal() {
      var today = new Date();
      vm.goal = {
        month: today.getMonth()+1,
        year: today.getFullYear(),
        goal: 0
      };
    }

    function close() {
      $uibModalInstance.dismiss("ok");
    }
  }
})();
