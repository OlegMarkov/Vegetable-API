<template>
<v-list :shaped="true" dense>
    <v-list-group v-model="listActive">
        <template v-slot:activator>
            <v-list-item-action>
                <v-icon>work</v-icon>
            </v-list-item-action>
            <v-list-item-content>
                <v-list-item-title>{{$t('employee.title')}}</v-list-item-title>
            </v-list-item-content>
        </template>
        <employee-in-list-view v-for="employee in employees" :currentEmployee="employee" :key="employee.id">
        </employee-in-list-view>
        <v-list-item>
            <v-list-item-action>
                <v-btn text small @click.stop="openEditDialog()">
                    <v-icon>add</v-icon>
                    <span>Add new</span>
                </v-btn>
            </v-list-item-action>
        </v-list-item>
    </v-list-group>
    <employee-edit v-model="editDialog" employeeId="00000000-0000-0000-0000-000000000000"></employee-edit>
</v-list>
</template>

<script>
import {
    mapGetters
} from "vuex";
import EmployeeInListView from '~/components/actions-panel/employee/employee-in-list.vue'
import EmployeeEdit from '~/components/actions-panel/employee/employee-edit.vue'

export default {
    data() {
        return {
            listActive: true,
            editDialog: false
        }
    },

    methods: {
        openEditDialog() {
            this.editDialog = true;
        }
    },

    computed: {
        ...mapGetters(["employees"])
    },

    components: {
        EmployeeInListView,
        EmployeeEdit
    }
}
</script>

<style scoped>
i.icon {
    cursor: pointer;
}

.list__group:before,
.list__group:after {
    content: none
}
</style>
