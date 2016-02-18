(function () {
  'use strict';

  angular
    .module('CRM')
    .controller('UserGroups', UserGroups);

  UserGroups.$inject = ['dataservice'];

  function UserGroups(dataservice) {
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
        .getAll()
        .then(function (groups) {
          vm.groups = groups;
          return vm.groups;
        });
    }

    function create() {
      vm.group.errMsg = undefined;
      dataservice.userGroups
        .create(vm.group)
        .then(function (group) {
          vm.groups.push(group);
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
      dataservice.userGroups
        .remove(id)
        .then(function () {
          _.remove(vm.stages, function (s) {
            return s.id == id;
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
