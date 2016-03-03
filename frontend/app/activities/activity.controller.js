(function(){
  'use strict';

  angular
    .module('CRM')
    .controller('Activity', Activity);

  Activity.$inject = ['dataservice', '$stateParams', '$state', 'authorization'];
  function Activity(dataservice, $stateParams, $state, auth){
    var vm = this;

    vm.categories = [];
    vm.users = [];
    vm.editing = false;

    vm.save = save;
    vm.remove = remove;
    vm.updateResponsible = updateResponsible;

    activate();

    function activate(){
      if($stateParams.id !== "new")
      {
        vm.editing = true;
        getActivity($stateParams.id);
      } else{
        var user = auth.getUser();
        vm.activity = {
          responsibleEmail: user.email,
          responsibleName: user.name
        };
      }
      getUsers();
      getCategories();
    }

    function getActivity(id){
      return dataservice.activities
        .getById(id)
        .then(function(a){
          vm.activity = a;
          vm.time = a.dueTime !== null;
        });
    }

    function save(){
      if(!vm.time) vm.activity.dueTime = null;
      if(vm.editing){
        dataservice.activities
          .update($stateParams.id, vm.activity)
          .then(function(){
            $state.go("Activities");
          });
      } else {
        dataservice.activities
          .create(vm.activity)
          .then(function(result){
            $state.go("Activity", {id: result.location});
          });
      }
    }

    function getCategories(){
      return dataservice.activityCategories
        .getAll()
        .then(function(c){
          vm.categories = c;
        });
    }

    function getUsers(){
      dataservice.users
        .getAll()
        .then(function(result){
          vm.users = result.data;
        });
    }

    function updateResponsible(resp){
      vm.responsible = undefined;
      vm.activity.responsibleEmail = resp.email;
      vm.activity.responsibleName = resp.name;
    }

    function remove(){
      return dataservice.activities
        .remove($stateParams.id)
        .then(function(){
          $state.go("Activities");
        });
    }
  }
})();
