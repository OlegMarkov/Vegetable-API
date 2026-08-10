<style>
  .obs-time {
    margin-left: 5px;
  }

  /***  calendar start  ***/

  .section-box {
    border: 1px solid #e5e5e5;
  }

  .obs-calendar-nav-buttons {
    float: right;
  }

  .section-calendar .section-box {
    padding: 0;
  }

  .section-box {
    background-color: #fff;
  }

  .calendar-busy .calendar-header {
    margin-top: 15px;
  }

  .calendar-busy .calendar-nav {
    padding: 0 15px;
    font-size: 14px;
  }

  .calendar-busy .calendar-nav .active-date {
    color: #333333;
    font-weight: 600;
    text-transform: uppercase;
  }

  .calendar-body {
    width: 100%;
  }

  .calendar-busy .calendar-body th {
    color: #757575;
    font-size: 11px;
    font-weight: 600;
    line-height: 1;
    text-transform: uppercase;
    padding: 13px 5px;
  }

  .calendar-busy .calendar-body th,
  .calendar-busy .calendar-body td {
    width: 14.2857%;
    text-align: center;
    vertical-align: middle;
  }


  .calendar-busy .calendar-body td {
    color: #1e87f0;
    font-size: 14px;
    font-weight: 400;
  }

  .calendar-busy .calendar-body td span {
    width: 36px;
    height: 36px;
    line-height: 36px;
    display: block;
    margin: 0 auto;
  }

  @media (max-width: 567px) {

    .calendar-busy .calendar-body td span {
      width: 25px;
      height: 25px;
      line-height: 25px;
    }

    .calendar-busy .calendar-header {
      margin-top: 5px;
    }

  }




  td.calendar-current-month.obs-calendar-day span {
    border-radius: 20px;
  }

  td.calendar-current-month.obs-calendar-day span:hover {
    cursor: pointer;
    background-color: #ebebeb
  }

  td.calendar-current-month.obs-calendar-day span.selected {
    background-color: #ebebeb
  }

  td.calendar-current-month span.obs-calendar-day-unavailable {
    color: #00000036;
  }

  td.calendar-current-month span.obs-calendar-day-unavailable:hover {
    cursor: default;
    background-color: white;
  }

  /***  calendar end  ***/

</style>

<template>
  <li>
    <div class="uk-text-center">
      <h3 class="">{{ $t('obs.date_title') }}</h3>
    </div>
    <div uk-grid class="uk-visible@s1">
      <div class="section-calendar uk-width-1-2@m">
        <div class="section-box" v-on:click.stop.prevent>
          <div class="calendar-busy">
            <div class="calendar-header">
              <div class="calendar-nav">
                <span class="active-date">
                  <span>{{monthFull}} {{year}}</span>
                </span>
                <a class="uk-icon-button obs-calendar-nav-buttons" v-on:click.stop.prevent v-on:click="nextMonth" title="Next" uk-icon="chevron-right"></a>
                <a class="uk-icon-button uk-margin-small-right obs-calendar-nav-buttons" v-on:click="previousMonth" title="Prev" uk-icon="chevron-left"></a>
              </div>
            </div>
            <table class="calendar-body">
              <thead class="calendar-thead">
                <tr>
                  <th v-for="weekDay in weekArray" :key="weekDay">{{weekDay}}</th>
                </tr>
              </thead>
              <tbody class="calendar-tbody">
                <tr>
                  <td v-for="day in calendar[0]" :key="day.day" class="calendar-current-month obs-calendar-day">
                    <span v-bind:class="day.class" v-bind:title="day.tooltip" v-on:click.stop.prevent v-on:click="daySelected(day)">{{day.day}}</span>
                  </td>
                </tr>
                <tr>
                  <td v-for="day in calendar[1]" :key="day.day" class="calendar-current-month obs-calendar-day">
                    <span v-bind:class="day.class" v-bind:title="day.tooltip" v-on:click.stop.prevent v-on:click="daySelected(day)">{{day.day}}</span>
                  </td>
                </tr>
                <tr>
                  <td v-for="day in calendar[2]" :key="day.day" class="calendar-current-month obs-calendar-day">
                    <span v-bind:class="day.class" v-bind:title="day.tooltip" v-on:click.stop.prevent v-on:click="daySelected(day)">{{day.day}}</span>
                  </td>
                </tr>
                <tr>
                  <td v-for="day in calendar[3]" :key="day.day" class="calendar-current-month obs-calendar-day">
                    <span v-bind:class="day.class" v-bind:title="day.tooltip" v-on:click.stop.prevent v-on:click="daySelected(day)">{{day.day}}</span>
                  </td>
                </tr>
                <tr>
                  <td v-for="day in calendar[4]" :key="day.day" class="calendar-current-month obs-calendar-day">
                    <span v-bind:class="day.class" v-bind:title="day.tooltip" v-on:click.stop.prevent v-on:click="daySelected(day)">{{day.day}}</span>
                  </td>
                </tr>
                <tr>
                  <td v-for="day in calendar[5]" :key="day.day" class="calendar-current-month obs-calendar-day">
                    <span v-bind:class="day.class" v-bind:title="day.tooltip" v-on:click.stop.prevent v-on:click="daySelected(day)">{{day.day}}</span>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
      <div class="uk-width-1-2@m">
        <div uk-margin class="uk-text-center">
          <h5>
           {{ $t('obs.date_available_time') }} {{selectedDateUI}}
          </h5>
          <button class="uk-button uk-button-default  uk-button-small obs-time" v-on:click.stop.prevent v-on:click="timeSelected(time)"
            v-for="time in selectedDay.availableTime" :key="time">{{time}}</button>
          <p v-if="selectedDay.availableTime && selectedDay.availableTime.length == 0">No available slots</p>
        </div>
      </div>
    </div>
  </li>
</template>

<script>
  import {
    mapGetters
  } from 'vuex'
  import moment from 'moment'
  import axios from 'axios'
  import _ from 'underscore'
  import {
    slots
  } from '~/mock/slots.js'

  export default {

    props: ['index'],

    data: function () {
      moment.locale(this.$store.state.locale);
      return {
        locale: this.$store.state.locale,
        year: moment().year(),
        month: moment().month(),
        calendar: [
          [],
          [],
          [],
          [],
          [],
          []
        ],
        selectedTime: null,
        selectedDay: {}
      }
    },

    computed: {
      ...mapGetters({
        selectedAddress: 'getSelectedAddress',
        selectedService: 'getSelectedService',
        selectedEmployee: 'getSelectedEmployee',
        selectedLocale: 'getSelectedLocale'
      }),
      weekArray() {
        if(this.$store.state.locale){
         return moment.weekdaysShort()
        }
      },
      selectedDateUI() {
        if (this.$store.state.locale && this.selectedDay)
          return moment(this.selectedDay.date).locale(this.$store.state.locale).format("dddd, LL");
        return '';
      },
      monthFull() {
        if(this.$store.state.locale){
          return moment.months()[this.month]
        }
      }
    },

    watch: {
      selectedAddress: function () {
        this.resetCalendar();
        this.initCalendar();
      },
      selectedService: function () {
        this.resetCalendar();
        this.initCalendar();
      },
      selectedEmployee: function () {
        this.resetCalendar();
        this.initCalendar();
      },
      selectedLocale: function(){
        moment.locale(this.$store.state.locale);
      }
    },

    methods: {
      resetCalendar() {
        this.calendar = [[], [], [], [], [], []];
        this.selectedDay = {};
        this.year = moment().year();
        this.month = moment().month();

      },
      initCalendar() {
        if (this.year && (this.month || this.month === 0) && this.selectedAddress && this.selectedService && this.selectedEmployee) {
          var daysInMonth = moment(this.year + "-" + (this.month + 1), "YYYY-MM").daysInMonth();
          var daysInPrevMonth = moment(this.year + "-" + (this.month == 0 ? 12 : this.month), "YYYY-MM").daysInMonth();
          var startOfMonth = moment(this.year + "-" + (this.month + 1), "YYYY-MM").startOf('month').day();
          var endOfMonth = moment(this.year + "-" + (this.month + 1), "YYYY-MM").endOf('month').day();
          var daysInWeek = 7;
          var startDate = this.getCalendarMonthStartDate(startOfMonth, daysInPrevMonth);
          var endDate = this.getCalendarMonthEndDate(endOfMonth, daysInMonth);
          //  return axios.get(process.env.APIBaseURL + 'schedule?service=' + $store.state.selectedService + '&employee=' + $store.state.selectedService + '&startDate=' + startDate + '&endDate=' + endDate + '&timeZone=UTC+3')

          var monthShift = daysInWeek - startOfMonth;

          var firstWeek = this.fillWeek(startOfMonth, 1, monthShift, daysInMonth, daysInPrevMonth, slots, 0);
          var secondWeek = this.fillWeek(0, (monthShift + 1), (monthShift + daysInWeek), daysInMonth, daysInPrevMonth,
            slots, 1);
          var thirdWeek = this.fillWeek(0, (monthShift + 1 + daysInWeek), (monthShift + 2 * daysInWeek), daysInMonth,
            daysInPrevMonth, slots, 2);
          var fourthWeek = this.fillWeek(0, (monthShift + 1 + 2 * daysInWeek), (monthShift + 3 * daysInWeek),
            daysInMonth, daysInPrevMonth, slots, 3);
          var fifthWeek = this.fillWeek(0, (monthShift + 1 + 3 * daysInWeek), (monthShift + 4 * daysInWeek),
            daysInMonth, daysInPrevMonth, slots, 4);
          var sixthWeek = this.fillWeek(0, (monthShift + 1 + 4 * daysInWeek), (monthShift + 5 * daysInWeek),
            daysInMonth, daysInPrevMonth, slots, 5);

          this.calendar[0] = firstWeek;
          this.calendar[1] = secondWeek;
          this.calendar[2] = thirdWeek;
          this.calendar[3] = fourthWeek;
          this.calendar[4] = fifthWeek;
          this.calendar[5] = sixthWeek;

          return;
        }

        this.calendar = [
          [],
          [],
          [],
          [],
          [],
          []
        ];
      },

      nextMonth() {
        if (this.month == 11) {
          this.month = 0;
          this.year += 1;
        } else {
          this.month += 1;
        }
        this.initCalendar();
      },
      previousMonth() {
        if (this.month == 0) {
          this.month = 11;
          this.year -= 1;
        } else {
          this.month -= 1;
        }
        this.initCalendar();
      },
      daySelected(day) {
        if (this.selectedDay || !_.isEmpty(this.selectedDay))
          this.selectedDay.class = this.selectedDay.class.replace(" selected", "");

        this.selectedDay = day;
        day.class += " selected";

      },
      timeSelected(time) {
        this.$store.commit('changeDate', this.selectedDay.date.toDate());
        this.$store.commit('changeTime', time);
        this.$store.commit('changeDateTime', moment(this.selectedDay.date.toDate()).locale(this.$store.state.locale).format("ddd, LL") +
          ' | ' + time);
        UIkit.tab('#steps').show(this.index + 1);
      },
      fillWeek(firstDayOfWeekInMonth, firstDayInWeek, lastDayInWeek, daysInMonth, daysInPrevMonth, slots, weekIndex) {
        var week = [];
        var nextMonthDay = 1;

        // fill previous month for first week
        for (var i = 0; i < firstDayOfWeekInMonth; i++) {

          var day = daysInPrevMonth - firstDayOfWeekInMonth + 1 + i;
          var year = this.month == 0 ? this.year - 1 : this.year;
          var month = this.month == 0 ? 11 : this.month - 1;
          var date = moment([year, month, day]);

          var slot = _.find(slots, function (slot) {
            return moment(date).isSame(slot.date, 'day');
          })

          var availabilityClass = (slot != null && slot.availableSlots.length > 0) ? '' :
            'obs-calendar-day-unavailable';

          week[i] = {
            date: date,
            day: day,
            class: 'obs-prev ' + availabilityClass,
            availableTime: slot != null ? slot.availableSlots : [],
            tooltip: availabilityClass == '' ? '' : 'No available slots',
            index: i
          }
        }

        // fill current month
        for (var i = firstDayInWeek; i <= lastDayInWeek; i++) {
          if (i <= daysInMonth) {
            var date = moment([this.year, this.month, i]);
            var slot = _.find(slots, function (slot) {
              return moment(date).isSame(slot.date, 'day');
            })

            var availabilityClass = (slot != null && slot.availableSlots.length > 0) ? '' :
              'obs-calendar-day-unavailable';

            week[firstDayOfWeekInMonth] = {
              date: date,
              day: i,
              class: 'current ' + availabilityClass,
              availableTime: slot != null ? slot.availableSlots : [],
              tooltip: availabilityClass == '' ? '' : 'No available slots',
              index: firstDayOfWeekInMonth
            };

            if (this.selectedDay == null || _.isEmpty(this.selectedDay) && i == moment().date()) {
              week[firstDayOfWeekInMonth].class += " selected";
              this.selectedDay = week[firstDayOfWeekInMonth];
            }
            firstDayOfWeekInMonth++;
          }
          // fill next month for last week
          else {
            if (week.length == 0) {
              return week;
            }

            var day = nextMonthDay;
            var year = this.month == 11 ? this.year + 1 : this.year;
            var month = this.month == 11 ? 0 : this.month + 1;

            var date = moment([year, month, day]);

            var slot = _.find(slots, function (slot) {
              return moment(date).isSame(slot.date, 'day');
            })

            var availabilityClass = (slot != null && slot.availableSlots.length > 0) ? '' :
              'obs-calendar-day-unavailable';

            week[firstDayOfWeekInMonth] = {
              date: date,
              day: nextMonthDay,
              class: 'obs-next ' + availabilityClass,
              availableTime: slot != null ? slot.availableSlots : [],
              tooltip: availabilityClass == '' ? '' : 'No available slots',
              index: firstDayOfWeekInMonth
            }
            nextMonthDay++;
            firstDayOfWeekInMonth++;
          }
        }
        return week;
      },
      getCalendarMonthStartDate(firstDayOfWeekInMonth, daysInPrevMonth) {
        if (firstDayOfWeekInMonth != 0) {
          var day = daysInPrevMonth - firstDayOfWeekInMonth + 1;
          var year = this.month == 0 ? this.year - 1 : this.year;
          var month = this.month == 0 ? 11 : this.month;
          return new Date(year, month, day);
        } else {
          return new Date(this.year, this.month, 1);
        }
      },
      getCalendarMonthEndDate(lastDayOfWeekInMonth, daysInMonth) {
        if (lastDayOfWeekInMonth != 6) {
          var day = 6 - lastDayOfWeekInMonth;
          var year = this.month == 11 ? this.year + 1 : this.year;
          var month = this.month == 11 ? 0 : this.month;
          return new Date(year, month, day);
        } else {
          return new Date(this.year, this.month, daysInMonth);
        }
      }
    }
  }

</script>
