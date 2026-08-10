<template>
<v-dialog v-model="serviceDialog" transition="dialog-bottom-transition" :overlay="false" max-width="800px">

    <template v-slot:activator="{ on: { click } }">
        <v-btn v-if="editMode" title="Edit" text icon v-on:click="click">
            <v-icon small>edit</v-icon>
        </v-btn>
        <v-btn v-if="!editMode" text small v-on:click="click">
            <v-icon>add</v-icon>
            <span>Add new</span>
        </v-btn>
    </template>

    <v-form ref="form" v-model="valid" :lazy-validation="false">
        <v-card>
            <v-toolbar color="white">
                <v-btn icon @click="closeDialog()">
                    <v-icon>keyboard_backspace</v-icon>
                </v-btn>
                <v-toolbar-title>{{$t('services.title')}}</v-toolbar-title>
                <v-spacer></v-spacer>
                <v-toolbar-items>
                    <v-tooltip left v-if="!valid" color="red">
                        <template v-slot:activator="{ on }">
                            <div v-on="on" class="my-4">
                                <v-btn disabled v-on="on" text>{{$t('general-settings.save')}}</v-btn>
                            </div>
                        </template>
                        <span>{{$t('general-settings.validation-summary')}}</span>
                    </v-tooltip>
                    <div v-else class="my-4">
                        <v-btn text @click="saveService()">{{$t('general-settings.save')}}</v-btn>
                    </div>
                </v-toolbar-items>
            </v-toolbar>
            <v-card-text>
                <v-container>
                    <v-row>
                        <v-col cols="12">
                            <v-list dense>
                                <v-list-item>
                                    <v-list-item-content>
                                        <v-row no-gutters>
                                            <v-col cols="2" align-self="center" class="text-left">
                                                <span>{{ $t('services.service-title') }}</span>
                                            </v-col>
                                            <v-col cols="10">
                                                <v-text-field v-model="service.title" :counter="50" required :rules="[required(), maxLength(50)]"></v-text-field>
                                                <small>{{ $t('services.service-title-description') }}</small>
                                            </v-col>
                                        </v-row>
                                    </v-list-item-content>
                                </v-list-item>
                                <v-list-item>
                                    <v-list-item-content>
                                        <v-row no-gutters>
                                            <v-col cols="2" align-self="center" class="text-left">
                                                <span>{{ $t('services.service-description') }}</span>
                                            </v-col>
                                            <v-col cols="10">
                                                <v-textarea v-model="service.description" :auto-grow="true" :rows="1" :counter="500" :rules="[maxLength(500)]"></v-textarea>
                                                <small>{{ $t('services.service-description-description') }}</small>
                                            </v-col>
                                        </v-row>
                                    </v-list-item-content>
                                </v-list-item>
                                <v-list-item>
                                    <v-list-item-content>
                                        <v-row no-gutters>
                                            <v-col cols="2" align-self="center" class="text-left">
                                                <span>{{ $t('services.service-duration') }}</span>
                                            </v-col>
                                            <v-col cols="2">
                                                <v-select required v-model.number="service.durationInMinutes" :items="[15, 30, 45, 60, 90]" :rules="[required()]">
                                                    <template v-slot:append>
                                                        <v-tooltip bottom>
                                                            <template v-slot:activator="{ on }">
                                                                <v-icon v-on="on">mdi-help-circle-outline</v-icon>
                                                            </template>
                                                            {{ $t('services.service-duration-description') }}
                                                        </v-tooltip>
                                                    </template>

                                                </v-select>
                                            </v-col>

                                        </v-row>
                                    </v-list-item-content>
                                </v-list-item>
                                <v-list-item>
                                    <v-list-item-content>
                                        <v-row no-gutters>
                                            <v-col cols="2" align-self="center" class="text-left">
                                                <span>{{ $t('services.service-cost') }}</span>
                                            </v-col>
                                            <v-col cols="2">
                                                <v-text-field type="number" label="Amount" v-model.number="service.cost"></v-text-field>
                                            </v-col>

                                        </v-row>
                                    </v-list-item-content>
                                </v-list-item>
                                <v-list-item>
                                    <v-list-item-content>
                                        <v-row no-gutters>
                                            <v-col cols="2" align-self="center" class="text-left">
                                                <span>{{ $t('services.service-color') }}</span>
                                            </v-col>
                                            <v-col cols="2">
                                                <color-selector :currentColor="service.color" @colorSelected="updateColor"></color-selector>
                                            </v-col>

                                        </v-row>
                                    </v-list-item-content>
                                </v-list-item>
                            </v-list>
                        </v-col>
                    </v-row>
                </v-container>
            </v-card-text>
        </v-card>
    </v-form>
</v-dialog>
</template>

<script>
import ColorSelector from '~/components/elements/color-selector.vue'

import {
    mapGetters
} from 'vuex';

import {
    currencyCodes
} from '~/mock/currencyCodes.js';

import {
    CREATE_SERVICE,
    UPDATE_SERVICE,
} from "@/store/actions.type";

let defaultService = {
    durationInMinutes: 0,
    title: '',
    description: '',
    cost: 0,
    usersCount: 1,
    currencyCode: '',
    color: 'red'
};

export default {
    data(context) {
        return {
            valid: true,
            service: defaultService,
            serviceDialog: false

        }
    },
    components: {
        ColorSelector
    },
    computed: {
        ...mapGetters([
            'getServiceById',
            'currentOwner'
        ]),
        currentServiceId() {
            return this.serviceId;
        },
    },

    created: function () {
        this.initService();
    },

    methods: {
        initService() {
            let service = (this.serviceId === undefined || this.serviceId === null || this.serviceId === '') ?
                defaultService :
                this.getServiceById(this.serviceId)

            this.service = {
                ...service
            }
        },

        closeDialog() {
            this.serviceDialog = false;
        },

        saveService() {
            if (this.service.id === undefined || this.service.id === null || this.service.id === '') {
                this.$store.dispatch(CREATE_SERVICE, this.service)
            } else {
                this.$store.dispatch(UPDATE_SERVICE, {
                    serviceId: this.service.id,
                    service: this.service
                })
            }
            this.serviceDialog = false;
        },

        updateColor(color) {
            this.service.color = color;
        },

        resetForm() {

        },
        required() {
            return value => !!value || this.$t("validation.required");
        },
        maxLength(max) {
            return value =>
                (value || "").length <= max || this.$t("validation.maxLength");
        }
    },
    props: ['serviceId', 'editMode']
}
</script>

<style>

</style>
