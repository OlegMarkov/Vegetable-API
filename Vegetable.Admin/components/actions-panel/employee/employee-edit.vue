<template>
<v-dialog v-model="show" fullscreen>

    <v-form ref="form" v-model="valid" :lazy-validation="false">
        <v-card>
            <v-toolbar color="white">
                <v-btn icon @click="closeDialog()">
                    <v-icon>keyboard_backspace</v-icon>
                </v-btn>
                <v-toolbar-title>{{$t('employee.employee-title')}}</v-toolbar-title>
                <v-spacer></v-spacer>
                <v-toolbar-items>
                    <v-tooltip left v-if="!valid" color="red">
                        <template v-slot:activator="{ on }">
                            <div v-on="on" class="my-4">
                                <v-btn disabled v-on="on" text>{{$t('employee.employee-save')}}</v-btn>
                            </div>
                        </template>
                        <span>{{$t('general-settings.validation-summary')}}</span>
                    </v-tooltip>
                    <div v-else class="my-4">
                        <v-btn text @click="saveEmployee()">{{$t('employee.employee-save')}}</v-btn>
                    </div>
                </v-toolbar-items>

            </v-toolbar>

            <v-card-text>
                <v-tabs vertical>
                    <v-tab justify="start">
                        <v-icon left>mdi-account</v-icon>
                        Profile
                    </v-tab>
                    <v-tab justify="start">
                        <v-icon left>mdi-lock</v-icon>
                        Schedules
                    </v-tab>

                    <v-tab-item>
                        <v-card flat>
                            <v-card-text>
                                <v-container>
                                    <v-row>
                                        <v-col cols="12">

                                            <v-list dense>
                                                <v-skeleton-loader :loading="loading" transition="scale-transition" height="100" width="1000" type="list-item-avatar-three-line">
                                                    <v-list-item>
                                                        <v-list-item-content>
                                                            <v-row no-gutters>
                                                                <v-col cols="1" align-self="center" class="text-left">
                                                                    <avatar-editor size="62" :color="currentColor" :currentImage="currentAvatar" :initials="initials" @savedImage="onImageChanged" />
                                                                </v-col>
                                                                <v-col cols="2">
                                                                    <label class="headline"> {{employee.firstName}} {{employee.lastName}}</label>
                                                                    <v-list-item-subtitle v-text="employee.description"></v-list-item-subtitle>
                                                                </v-col>
                                                            </v-row>
                                                        </v-list-item-content>

                                                    </v-list-item>
                                                </v-skeleton-loader>

                                                <v-list-item>
                                                    <v-list-item-content>
                                                        <v-row no-gutters>
                                                            <v-col cols="2" align-self="center" class="text-left">
                                                                <v-skeleton-loader :loading="loading" transition="scale-transition" height="50" type="list-item">
                                                                    <span>{{ $t('employee.employee-firstName') }}</span>
                                                                </v-skeleton-loader>
                                                            </v-col>

                                                            <v-col cols="10">
                                                                <v-skeleton-loader :loading="loading" transition="scale-transition" height="50" type="list-item">
                                                                    <v-text-field v-model="employee.firstName" :rules="[required(), maxLength(50)]"></v-text-field>
                                                                </v-skeleton-loader>
                                                            </v-col>

                                                        </v-row>
                                                    </v-list-item-content>
                                                </v-list-item>

                                                <v-list-item>
                                                    <v-list-item-content>
                                                        <v-row no-gutters>
                                                            <v-col cols="2" align-self="center" class="text-left">
                                                                <v-skeleton-loader :loading="loading" transition="scale-transition" height="50" type="list-item">
                                                                    <span>{{ $t('employee.employee-lastName') }}</span>
                                                                </v-skeleton-loader>
                                                            </v-col>
                                                            <v-col cols="10">
                                                                <v-skeleton-loader :loading="loading" transition="scale-transition" height="50" type="list-item">
                                                                    <v-text-field v-model="employee.lastName" :rules="[required(), maxLength(50)]"></v-text-field>
                                                                    <small>{{ $t('employee.employee-lastName-description') }}</small>
                                                                </v-skeleton-loader>
                                                            </v-col>
                                                        </v-row>
                                                    </v-list-item-content>
                                                </v-list-item>

                                                <v-list-item>
                                                    <v-list-item-content>
                                                        <v-row no-gutters>
                                                            <v-col cols="2" align-self="center" class="text-left">
                                                                <v-skeleton-loader :loading="loading" transition="scale-transition" height="50" type="list-item">
                                                                    <span>{{ $t('employee.employee-description') }}</span>
                                                                </v-skeleton-loader>
                                                            </v-col>
                                                            <v-col cols="10">
                                                                <v-skeleton-loader :loading="loading" transition="scale-transition" height="50" type="list-item">
                                                                    <v-text-field v-model="employee.description" :rules="[ maxLength(100)]"></v-text-field>
                                                                    <small>{{ $t('employee.employee-description-description') }}</small>
                                                                </v-skeleton-loader>
                                                            </v-col>
                                                        </v-row>
                                                    </v-list-item-content>
                                                </v-list-item>

                                                <v-list-item>
                                                    <v-list-item-content>
                                                        <v-row no-gutters>
                                                            <v-col cols="2" align-self="center" class="text-left">
                                                                <v-skeleton-loader :loading="loading" transition="scale-transition" height="50" type="list-item">
                                                                    <span>{{ $t('employee.employee-availableServices') }}</span>
                                                                </v-skeleton-loader>
                                                            </v-col>
                                                            <v-col cols="10">
                                                                <v-skeleton-loader :loading="loading" transition="scale-transition" height="50" type="list-item">
                                                                    <v-select required v-model="employee.address" item-text="description" item-value="id" :items="addresses" return-object>
                                                                    </v-select>
                                                                    <small>{{ $t('employee.employee-availableServices-description') }}</small>
                                                                </v-skeleton-loader>
                                                            </v-col>
                                                        </v-row>
                                                    </v-list-item-content>
                                                </v-list-item>

                                                <v-list-item v-show="services.length > 0">
                                                    <v-list-item-content>
                                                        <v-row no-gutters>
                                                            <v-col cols="2" align-self="center" class="text-left">
                                                                <v-skeleton-loader :loading="loading" transition="scale-transition" height="100" type="list-item">
                                                                    <span>{{ $t('services.service-description') }}</span>
                                                                </v-skeleton-loader>
                                                            </v-col>
                                                            <v-col cols="10">
                                                                <v-skeleton-loader :loading="loading" transition="scale-transition" height="100"  type="list-item">
                                                                    <v-container>
                                                                        <v-row>
                                                                            <v-col cols="12" md="3" v-for="service in services" :key="service.id">
                                                                                <v-checkbox v-model="selectedServices" :label="service.title" :value="service"></v-checkbox>
                                                                            </v-col>
                                                                        </v-row>
                                                                    </v-container>
                                                                </v-skeleton-loader>
                                                            </v-col>
                                                        </v-row>
                                                    </v-list-item-content>
                                                </v-list-item>
                                               
                                                    <v-list-item>
                                                        <v-list-item-content>
                                                            <v-row no-gutters>
                                                                <v-col cols="2" align-self="center" class="text-left">
                                                                     <v-skeleton-loader :loading="loading" transition="scale-transition" height="50"  type="list-item">
                                                                    <span>{{ $t('services.service-color') }}</span>
                                                                     </v-skeleton-loader>
                                                                </v-col>
                                                                <v-col cols="2">
                                                                     <v-skeleton-loader :loading="loading" transition="scale-transition" height="50" type="avatar">
                                                                    <color-selector :currentColor="currentColor" @colorSelected="updateColor"></color-selector>
                                                                     </v-skeleton-loader>
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
                    </v-tab-item>
                    <v-tab-item>
                        <v-card flat>
                            <v-card-text>
                                 <v-container>
                                    <v-row>
                                        <v-col cols="12">
                                        </v-col>
                                    </v-row>
                                 </v-container>
                            </v-card-text>
                        </v-card>
                    </v-tab-item>
                </v-tabs>
            </v-card-text>

        </v-card>
    </v-form>
</v-dialog>
</template>

<script>
import ColorSelector from '~/components/elements/color-selector.vue';

import {
    EmployeesService,
    ImagesService
} from "@/common/api.service";

import AvatarEditor from '~/components/elements/avatar-editor.vue'

import axios from "axios";

import {
    CREATE_EMPLOYEE,
    UPDATE_EMPLOYEE
} from "@/store/actions.type";

import {
    mapGetters
} from 'vuex';

import _ from "underscore";

export default {
    data() {
        return {
            dialog: false,
            employee: {},
            selectedServices: [],
            avatar: null,
            valid: true,
            loading: true
        }
    },
    components: {
        AvatarEditor,
        ColorSelector
    },
    created() {

    },
    computed: {
        show: {
            get() {
                return this.value
            },
            set(value) {
                this.$emit('input', value)
            }
        },
        currentColor: {
            get() {
                return (this.employee.color === undefined || this.employee.color === null || this.employee.color === '') ? "red" : this.employee.color;
            },
            set(value) {
                this.employee.color = value;
            }
        },

        currentAvatar: {
            get() {
                return (this.employee.avatar === undefined || this.employee.avatar === null || this.employee.avatar === '') ? "" : this.employee.avatar;
            },
            set(value) {
                this.employee.avatar = value;
            }
        },

        ...mapGetters([
            'addresses', 'services'
        ]),

        initials() {
            let first = (this.employee.firstName === undefined || this.employee.firstName === null || this.employee.firstName === '') ? "" : this.employee.firstName.charAt(0);
            let last = (this.employee.lastName === undefined || this.employee.lastName === null || this.employee.lastName === '') ? "" : this.employee.lastName.charAt(0);
            return first + last;
        }

    },

    watch: {
        // whenever question changes, this function will run
        show: function (newShowValue, oldShowValue) {
            if (newShowValue) {
                this.fetchEmployee(this.employeeId);
            } else {
                this.employee = {};
                this.loading = true;
            }
        }
    },

    methods: {
        async fetchEmployee(id) {
            await axios
                .get(`/owner/employee/${id}`)
                .then(response => {
                    this.employee = JSON.parse(JSON.stringify(response.data));
                    let employeeServices = response.data.employeeServices.map(a => a.service);
                    this.services.forEach(function (obj, index) {
                        var currentService = employeeServices.find(e => e.id === obj.id);
                        if (currentService != null) {
                            this.selectedServices.push(obj);
                        }
                    }, this);

                    this.loading = false;

                });
        },
        onImageChanged(image) {
            this.avatar = image;
        },
        saveEmployee() {
            var employeeServices = [];
            var currentEmployee = {
                ...this.employee
            };
            delete currentEmployee.employeeServices;

            this.selectedServices.forEach(function (obj, index) {
                employeeServices.push({
                    service: obj,
                    employee: currentEmployee
                });
            }, this);

            this.employee.employeeServices = employeeServices;

            if (this.avatar != null) {
                this.saveAvatar(this.avatar.avatar, this.employee.firstName + this.employee.lastName)
                    .then(response => {
                        this.employee.avatar = response.data;
                        this.save();
                        this.show = false;
                    });
            } else {
                this.save();
                this.show = false;
            }
        },

        updateColor(color) {
            this.employee.color = color;
        },

        closeDialog() {
            this.show = false;
        },

        save() {
            if (this.employee.id === undefined || this.employee.id === null || this.employee.id === '00000000-0000-0000-0000-000000000000') {
                this.$store.dispatch(CREATE_EMPLOYEE, this.employee)
            } else {
                this.$store.dispatch(UPDATE_EMPLOYEE, {
                    employeeId: this.employee.id,
                    employee: this.employee
                })
            }
        },

        async saveAvatar(avatarBase64, fileName) {
            let image = {
                ImageBase64: avatarBase64,
                Name: fileName
            }
            return await ImagesService.add(image);
        },

        required() {
            return value => !!value || this.$t("validation.required");
        },
        maxLength(max) {
            return value =>
                (value || "").length <= max || this.$t("validation.maxLength");
        }
    },
    props: {
        value: Boolean,
        employeeId: String
    }
}
</script>

<style scoped>
.v-dialog>.v-card>.v-card__text {
    padding-top: 16px;
}

.v-form {
    height: 100%;
}
</style>
