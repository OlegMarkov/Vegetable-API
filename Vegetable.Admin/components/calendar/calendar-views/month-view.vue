<template>

<div class="full-calendar-body">
    <div class="weeks">
        <strong class="week" v-for="(dayIndex, index) in 7" :key='index'>{{ (dayIndex - 1) | weekDayName(1, appLocale) }}</strong>
    </div>
    <div class="dates" ref="dates">
        <div class="week-row" v-for="(week, index) in Weeks" :key='index'>
            <div class="day-cell" v-for="(day, index) in week" :class="{'today' : day.isToday, 'current-month' : day.isCurrentMonth, 'weekend': day.isWeekEnd}" :key='index' v-on:click.self="selectDay(day.date)">
                <div class="row" >
                    <div class="col-sm-6">
                    </div>
                    <div class="col-sm-6">
                        <p class="day-number">{{ day.date.format('D') }}</p>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

</template>

<script>

let moment = require('moment')
export default {
  props: {
    currentDate: {}
  },

  methods: {
    getMonthViewStartDate (date, firstDay) {
      firstDay = parseInt(firstDay)
      let start = moment(date).locale(this.appLocale)
      let startOfMonth = moment(start.startOf('month'))
      start.subtract(startOfMonth.day(), 'days')
      if (startOfMonth.day() < firstDay) {
        start.subtract(7, 'days')
      }
      start.add(firstDay, 'days')
      return start
    },

    getDayObject (monthMomentObject, dayIndex, currentMonth) {
      return {
        isToday: monthMomentObject.isSame(moment(), 'day'),
        isCurrentMonth: monthMomentObject.isSame(currentMonth, 'month'),
        weekDay: dayIndex,
        isWeekEnd: (dayIndex === 5 || dayIndex === 6),
        date: moment(monthMomentObject)
      }
    },

    currentMonth () {
      return moment(this.currentDate).startOf('month')
    },

    selectDay (day) {
      this.$root.$emit('CHANGE_SCHEDULE', day)
    }
  },
  computed: {
    Weeks () {
      let monthMomentObject = this.getMonthViewStartDate(this.currentMonth(), 1)
      let weeks = []
      let week = []
      let daysInCurrentMonth = this.currentMonth().daysInMonth()
      for (let weekIndex = 0; weekIndex < 5; weekIndex++) {
        week = []
        for (let dayIndex = 0; dayIndex < 7; dayIndex++) {
          week.push(this.getDayObject(monthMomentObject, dayIndex, this.currentMonth()))
          monthMomentObject.add(1, 'day')
        }
        weeks.push(week)
      }
      let diff = daysInCurrentMonth - weeks[4][6].date.format('D')
      if (diff > 0 && diff < 3) {
        week = []
        for (let dayIndex = 0; dayIndex < 7; dayIndex++) {
          week.push(this.getDayObject(monthMomentObject, dayIndex))
          monthMomentObject.add(1, 'day')
        }
        weeks.push(week)
      }
      return weeks
    },
    appLocale: function () {
      return this.$store.state.owner.locale
    }
  },
  filters: {
    weekDayName (weekday, firstDay, locale) {
      firstDay = parseInt(firstDay)
      var localMoment = moment().locale(locale)
      return localMoment.localeData().weekdaysShort()[(weekday + firstDay) % 7]
    }
  }
}
</script>

<style>

.full-calendar-body {
    margin-top: 20px;
}

.weeks {
    display: flex;
    border-top: 1px solid #e0e0e0;
    border-bottom: 1px solid #e0e0e0;
    border-left: 1px solid #e0e0e0;
}

.week {
    flex: 1;
    padding: 5px;
    text-align: center;
    border-right: 1px solid #e0e0e0;
}

.dates {
    position: relative;
}

.week-row {
    width: 100%;
    border-left: 1px solid #e0e0e0;
    display: flex;
    cursor: pointer;
}

.day-cell {
    flex: 1;
    min-height: 112px;
    padding: 4px;
    border-right: 1px solid #e0e0e0;
    border-bottom: 1px solid #e0e0e0;
    background: rgba(147, 147, 147, 0.1);
}

.day-number {
    text-align: right;
    color: rgba(0, 0, 0, .25);
    font-size: 1em;
    padding: 5px;
}

.current-month {
    background: #fff;
}

.current-month p {
    color: rgba(0, 0, 0, .5);
    font-size: 1.5em;
}

.selected-day p {
    font-size: 2.4em;
    font-weight: bolder;
}

.weekend p {
    color: rgba(210, 2, 2, 0.6);
}

.today {
    background-color: #e8fde7;
}

.today p {
    font-weight: bolder;
}

</style>
