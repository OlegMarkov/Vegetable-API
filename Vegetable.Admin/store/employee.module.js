import _ from "underscore";

import {
    EmployeesService
} from "@/common/api.service";

import {
    CREATE_EMPLOYEE,
    UPDATE_EMPLOYEE,
    DELETE_EMPLOYEE,
    FETCH_EMPLOYEES
} from "./actions.type";

import {
    SET_EMPLOYEES
} from "./mutations.type";

const initialState = {
    employees: [],
    selectedEmployees: []
};

export const state = { ...initialState };

export const actions = {
    async [FETCH_EMPLOYEES](context) {
        const { data } = await EmployeesService.fetch();
        context.commit(SET_EMPLOYEES, data);
        return data;
    },

    // async [FETCH_EMPLOYEE](context) {
    //     const { data } = await ServicesService.fetch();
    //     context.commit(SET_EMPLOYEES, data);
    //     return data;
    // },

    async [CREATE_EMPLOYEE](context, employee) {
        await EmployeesService.create(employee)
        context.dispatch(FETCH_EMPLOYEES);
    },

    async [UPDATE_EMPLOYEE](context, payload) {
        await EmployeesService.update(payload.employeeId, payload.employee)
        context.dispatch(FETCH_EMPLOYEES);
    },

    async [DELETE_EMPLOYEE](context, employeeId) {
        await EmployeesService.delete(employeeId)
        context.dispatch(FETCH_EMPLOYEES);
    }

};

export const mutations = {
    [SET_EMPLOYEES](state, employees) {
        var selectedEmployees = [];
        _.each(employees, function (employee) {
            var currentEmployee = state.employees.find(e => employee.id === e.id);
            selectedEmployees.push({
                id: employee.id,
                checked: (currentEmployee != null ? currentEmployee.checked : true),
                employee: employee
            });
        });

        state.employees = selectedEmployees;
    }

};

const getters = {
    employees: state => state.employees,
    getEmployeesById: state => id => state.employees.find(employee => employee.id === id)
};

export default {
    state,
    actions,
    mutations,
    getters
};