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
    vm.addInterest = addInterest;
    vm.removeInterest = removeInterest;
    vm.updateCompany = updateCompany;
    vm.removeCompany = removeCompany;
    vm.contactSelected = contactSelected;
    vm.primaryContactSelected = primaryContactSelected;
    vm.removeContact = removeContact;
    vm.commentDataAccess = dataservice.activityComments;
    vm.commentArgs = {activityId: $stateParams.id};
    vm.timeChange = timeChange;

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
        .get(id)
        .then(function(a){
          vm.activity = a;
          vm.time = a.dueTimeStart !== null;
          getEmployees();
        });
    }

    function getEmployees(){
      if(vm.activity.companyId){
        dataservice.companyEmployees({companyId: vm.activity.companyId})
          .then(function(data){
            vm.employees = data;
            vm.employees = filterList(vm.employees, vm.activity.secondaryContacts);
          });
      } else {
        vm.employees = [];
      }
    }

    function save(){
      if(!vm.time) vm.activity.dueTime = null;
      if(vm.editing){
        dataservice.activities
          .update($stateParams.id, vm.activity)
          .then(function(){
            $state.go("Activities");
          }, handleRequestError);
      } else {
        dataservice.activities
          .create(vm.activity)
          .then(function(result){
            $state.go("Activity", {id: result.location});
          }, handleRequestError);
      }
    }

    function getCategories(){
      return dataservice.activityCategories
        .get()
        .then(function(c){
          vm.categories = c;
        });
    }

    function getUsers(){
      dataservice.users
        .get()
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

    function updateCompany(company){
      vm.activity.companyName = company.name;
      vm.activity.companyId = company.id;
      vm.activity.secondaryContacts = [];
      getEmployees();
    }

    function removeCompany(company){
      vm.activity.companyName = undefined;
      vm.activity.companyId = undefined;
      getEmployees();
      vm.activity.secondaryContacts = [];
    }

    function contactSelected(contact){
      vm.contact = undefined;
      vm.activity.secondaryContacts.push(contact);
      vm.employees = filterList(vm.employees, vm.activity.secondaryContacts);
    }

    function removeContact(contact){
      vm.employees.push(contact);
      _.remove(vm.activity.secondaryContacts, function(c){return c.id === contact.id;});
    }

    function addInterest(responsible){
      vm.activity.secondaryResponsibles.push(responsible);
      vm.interest = null;
    }

    function removeInterest(responsibility) {
      _.remove(vm.activity.secondaryResponsibles, function(r){return r.email === responsibility.email;});
    }

    function primaryContactSelected(contact){
      vm.activity.primaryContactId = contact.id;
      vm.activity.primaryContactName = contact.name;
      vm.primaryContact = null;
    }

    function timeChange(){
      if(!vm.time){
        vm.activity.dueTimeStart = null;
        vm.activity.dueTimeEnd = null;
      }
    }

    function filterList(list, compareList){
      return _.differenceBy(list, compareList, function(i){
        return i.id;
      });
    }
    function handleRequestError(err){
      if(err.status === 400){
        vm.modelState = err.data;
      }
    }
  }
})();
