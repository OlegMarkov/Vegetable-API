<template>
<v-tooltip bottom="">
    <template v-slot:activator="{ on }">
        <v-list-item link @mouseover="mousehover = true" @mouseleave="mousehover = false" v-on="on" v-bind:style="[isChecked ? activeStyle : notCheckedStyle]" @click="onItemClicked()">
          <template>
            <v-list-item-avatar :color="employeeInfo.employee.color">
                <v-img v-if="isImageLoaded" :src="employeeInfo.employee.avatar"></v-img>
                <span v-else class="white--text">{{employeeInfo.employee.initials}}</span>
            </v-list-item-avatar>

            <v-list-item-content>
                <v-list-item-title v-html="employeeInfo.employee.fullName"></v-list-item-title>
                <v-list-item-subtitle v-html="employeeInfo.employee.description"></v-list-item-subtitle>
            </v-list-item-content>
             <v-list-item-action v-show="mousehover">
                <v-btn title="Edit" text icon @click="editEmployee">
                    <v-icon small>edit</v-icon>
                </v-btn>
            </v-list-item-action>
            <v-list-item-action v-show="mousehover">
                <v-btn title="Delete" text icon @click="deleteEmployee">
                    <v-icon small>delete</v-icon>
                </v-btn>
            </v-list-item-action>   
          </template>        
        </v-list-item>
        <employee-edit v-model="editDialog" :employeeId="employeeInfo.id"></employee-edit>
    </template>
    <span>{{employeeInfo.employee.fullName}} <br/> {{employeeInfo.employee.description}}</span>
</v-tooltip>
</template>

<script>
import {
    UPDATE_EMPLOYEE,
    DELETE_EMPLOYEE
} from "@/store/actions.type";
import EmployeeEdit from '~/components/actions-panel/employee/employee-edit.vue'
export default {
    data() {
        return {
            mousehover: false,
            editDialog: false
        }
    },
    components: {
        EmployeeEdit
    },
    computed: {
        employeeInfo() {
            return this.currentEmployee
        },

        isChecked: {
          get() {
            return this.currentEmployee.checked;
          },
          set(value) {
            this.currentEmployee.checked = value;
          }
        },

        isImageLoaded() {
            return !(this.employeeInfo.employee.avatar === undefined || this.employeeInfo.employee.avatar === null || this.employeeInfo.employee.avatar === '');
        },
        activeStyle() { 
          return {
          "border-left-style": 'solid',          
          "border-left-width": '10px',
          "border-left-color": this.employeeInfo.employee.color,

          }
        },
        notCheckedStyle() { 
          return {
          "border-left-style": 'solid',          
          "border-left-width": '10px',
          "border-left-color": 'white'

          }
        }
    },
    methods: {
        deleteEmployee() {
            this.$store.dispatch(DELETE_EMPLOYEE, this.employeeInfo.id);
        },
        editEmployee() {
             this.editDialog = true;
        },
        onItemClicked(){
           this.isChecked = !this.isChecked;
        }
    },
    props: ['currentEmployee']
}
</script>

<style scoped>
.v-list-item__action {
    margin: 0;
}

.v-v-list-item__title {
    margin-left: 15px;
}

.v-list-item__action:last-of-type:not(:only-child) {
    margin: 0;
}

.v-application .headline,
.v-application .title {
    font-size: 1rem;
}
.styled {
  border-left-style: solid;
  border-left-width: 3px;
}
</style>
