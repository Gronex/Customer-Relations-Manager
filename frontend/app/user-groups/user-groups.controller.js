(function () {
  'use strict';

  angular
    .module('CRM')
    .controller('UserGroups', UserGroups);

  UserGroups.$inject = ['dataservice', 'warning'];

  function UserGroups(dataservice, warning) {
    var vm = this;

    vm.group = { name: ""};
    vm.groups = [];

    vm.save = save;
    vm.remove = remove;
    vm.create = create;

    activate();

    function activate() {
      getGroups();
    }

    function getGroups() {
      dataservice.userGroups
        .get()
        .then(function (groups) {
          vm.groups = groups;
          return vm.groups;
        });
    }

    function create() {
      vm.group.errMsg = undefined;
      dataservice.userGroups
        .create(vm.group)
        .then(function (data) {
          vm.groups.push(data.data);
          vm.group.name = "";
        }, function (err) { handleError(err, vm.group); });
    }

    function save(group) {
      group.errMsg = undefined;
      if(group.saved || group.saved == undefined) return;
      dataservice.userGroups
        .update(group.id, group)
        .then(function () {
          group.saved = true;
        }, function (err) { handleError(err, group); });
    }

    function remove(id) {
      warning.warn(["This group may be connected to other things, and deleting it will remove it from these as well.", "Are you sure you want to continue?"]).then(function(){
        dataservice.userGroups
          .remove(id)
          .then(function () {
            _.remove(vm.groups, function (s) {
              return s.id == id;
            });
          });
      });
    }

    function handleError(err, target) {
      switch (err.status) {
        case 400:

          break;
        case 409:
          target.errMsg = "Group with name: '" + err.data.name + "' already exists";
          break;
        default:
      }
    }
  }
})();
