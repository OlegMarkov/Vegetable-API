import _ from "underscore";

import {
    SettingsService
} from "@/common/api.service";

import {
    FETCH_CURRENCIES
} from "./actions.type";

import {
    SET_CURRENCIES
} from "./mutations.type";

const initialState = {
    currencies: []
};

export const state = { ...initialState };

export const actions = {
    async [FETCH_CURRENCIES](context) {
        const { data } = await SettingsService.getCurrencies();
        context.commit(SET_CURRENCIES, data);
        return data;
    }
};

export const mutations = {
    [SET_CURRENCIES](state, currencies) {
        state.currencies = currencies;
    }
};

const getters = {
    currencies: state => state.currencies
};

export default {
    state,
    actions,
    mutations,
    getters
};