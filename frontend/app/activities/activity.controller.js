(function(){
  'use strict';

  angular
    .module('CRM')
    .controller('Activity', Activity);

  Activity.$inject = ['person', 'company', 'activity', 'dataservice', '$stateParams', '$state', 'authorization', 'warning'];
  function Activity(person, company, activity, dataservice, $stateParams, $state, auth, warning){
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
    vm.removePrimaryContact = removePrimaryContact;
    vm.contactSelected = contactSelected;
    vm.primaryContactSelected = primaryContactSelected;
    vm.removeContact = removeContact;
    vm.commentDataAccess = dataservice.activityComments;
    vm.commentArgs = {activityId: $stateParams.id};
    vm.timeChange = timeChange;

    activate();

    function activate(){
      if($state.is("Activities.edit"))
      {
        vm.editing = true;
        vm.activity = activity;
        vm.time = activity.dueTimeStart !== null;
        getEmployees();
      } else{
        var user = auth.getUser();
        vm.activity = {
          responsibleEmail: user.email,
          responsibleName: user.name,
          secondaryContacts: [],
          secondaryResponsibles: [],
          primaryContactId: person.id,
          primaryContactName: person.name,
          companyId: company.id,
          companyName: company.name
        };
      }
      getUsers();
      getCategories();
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
            $state.go("Activities.list");
          }, handleRequestError);
      } else {
        dataservice.activities
          .create(vm.activity)
          .then(function(result){
            $state.go("Activities.edit", {id: result.location});
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
      return warning.warn(["You are about to delete the activity '" + vm.activity.name + "', this operation cannot be undone.", "Are you sure you want to continue?"]).then(function(){
        return dataservice.activities
          .remove($stateParams.id)
          .then(function(){
            $state.go("Activities.list");
          });
      });
    }

    function updateCompany(company){
      vm.activity.companyName = company.name;
      vm.activity.companyId = company.id;
      vm.activity.secondaryContacts = [];
      vm.activity.primaryContactId = undefined;
      vm.activity.primaryContactName = undefined;
      getEmployees();
    }

    function removeCompany(){
      vm.activity.primaryContactId = undefined;
      vm.activity.primaryContactName = undefined;
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

    function removePrimaryContact(){
      if(vm.activity.secondaryContacts.length > 0){
        var contact = vm.activity.secondaryContacts.splice(0,1)[0];
        vm.activity.primaryContactName = contact.name;
        vm.activity.primaryContactId = contact.id;
        vm.employees.push({
          name: vm.activity.primaryContactName,
          id: vm.activity.primaryContactId
        });
      } else{
        vm.activity.primaryContactName = undefined;
        vm.activity.primaryContactId = undefined;
      }
    }
  }
})();
