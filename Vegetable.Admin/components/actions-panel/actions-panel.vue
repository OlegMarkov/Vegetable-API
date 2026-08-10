<template>
  <v-list dense>
    <general-settings></general-settings>
    <v-divider></v-divider>
    <employees-list-view
      @editEmployee="editEmployeeClick"
      @deleteEmployee="deleteEmployeeClick"
    >
    </employees-list-view>
    <v-divider></v-divider>
    <services-list-view 
      @editService="editServiceClick"
      @deleteService="deleteServiceClick"
    >
    </services-list-view>
    <v-divider></v-divider>
    <employee-edit-view 
      @onCloseDialog="employee_dialog = false"
      @onEmployeeSave="onEmployeeSave"
      :employeeDialog="employee_dialog"
      :employeeId="currentEmployeeId"
    >
    </employee-edit-view>
    
    <user-profile></user-profile>
    <v-divider></v-divider>
  </v-list>
</template>

<script>
import axios from 'axios'
import _ from 'underscore'
import GeneralSettings from '~/components/actions-panel/general-settings.vue'
import EmployeesListView from '~/components/actions-panel/employee/employees-list.vue'
import EmployeeEditView from '~/components/actions-panel/employee/employee-edit.vue'
import ServicesListView from '~/components/actions-panel/service/services-list.vue'
import ServiceEditView from '~/components/actions-panel/service/service-edit.vue'
import UserProfile from '~/components/actions-panel/user-profile.vue'
import AvatarEditor from '~/components/elements/avatar-editor.vue'
import Constants from '~/config.js'

import {
    CREATE_SERVICE,
    UPDATE_SERVICE,
    DELETE_SERVICE 
} from "@/store/actions.type";

export default {
  data () {
    return {
      createNewEmployee: false,
      currentEmployeeId: null,
      employee_dialog: false,
      createNewService: false,
      currentServiceId: null,
      service_dialog: false
    }
  },

  components: {
    GeneralSettings,
    EmployeesListView,
    ServicesListView,
    EmployeeEditView,
    ServiceEditView,
    UserProfile,
    AvatarEditor
  },

  methods: {
    // Employee
    onEmployeeSave (employee) {
      this.employee_dialog = false

      if (employee.id === undefined || employee.id === null || employee.id === '') {
        axios.post(Constants.ApiOwnerUrl + 'employee', employee)
          .then((response) => {
            let newEmployee = response.data
            this.$store.state.owner.employees.push(newEmployee)
          })
      } else {
        this.$store.commit('updateEmployee', employee)
        axios.put(Constants.ApiOwnerUrl + 'employee/' + employee.id, employee)
      }
    },
    editEmployeeClick (employeeId) {
      this.currentEmployeeId = employeeId
      this.employee_dialog = true
    },
    deleteEmployeeClick (employeeId) {
      axios.delete(Constants.ApiOwnerUrl + 'employee/' + employeeId)
        .then((response) => {
          let index = _.findIndex(this.$store.state.owner.employees, (employee) => employee.id === employeeId)
          if (index > -1) {
            this.$store.state.owner.employees.splice(index, 1)
          }
        })
    },

    // Service
    onServiceSave (service) {
      this.service_dialog = false
      if(service.id === undefined || service.id === null || service.id === ''){
        this.$store.dispatch(CREATE_SERVICE, service)
      }
      else{
        this.$store.dispatch(UPDATE_SERVICE, {serviceId: service.id, service:  service})
      }
      //axios.post(Constants.ApiOwnerUrl + 'service', service)
    },
    
    deleteServiceClick (serviceId) {
      this.$store.dispatch(DELETE_SERVICE, serviceId)

      // let index = _.findIndex(this.$store.state.owner.services, (service) => service.id === serviceId)
      // if (index > -1) {
      //   this.$store.state.owner.services.splice(index, 1)
      // }
    }
  },

  props: {

  }
}
</script>

<style>

</style>