const date = new Date();
var  year = new Date().getFullYear();
const renderCalendar = () => {

    date.setDate(1);

    const monthDays = document.querySelector(".days");

    const lastDay = new Date(
        date.getFullYear(),
        date.getMonth() + 1,
        0
    ).getDate();
    const prevLastDay = new Date(
        date.getFullYear(),
        date.getMonth(),
        0
    ).getDate();

    const firstDayIndex = date.getDay();

    const lastDayIndex = new Date(
        date.getFullYear(),
        date.getMonth(),
        0
    ).getDay();

    const nextDays = 7 - lastDayIndex - 1;

    const months = [
        "Janeiro",
        "Fevereiro",
        "Março",
        "Abril",
        "Maio",
        "Junho",
        "Julho",
        "Agosto",
        "Setembro",
        "Outubro",
        "Novembro",
        "Dezembro",
    ];

    document.querySelector(".date h1").innerHTML = months[date.getMonth()] + " " + year;

    let days = "";

    for (let x = firstDayIndex; x > 0; x--) {
        days += `<div data-goto='${year - 1}-${checkMoth(date.getMonth(), 1)}-${checkDay(prevLastDay - x + 1)}' class="prev-date dayGet">${prevLastDay - x + 1}</div>`;
    }

    for (let i = 1; i <= lastDay; i++) {
        if (
            i === new Date().getDate() &&
            date.getMonth() === new Date().getMonth()
        ) {
            days += `<div data-goto='${year}-${checkMoth(date.getMonth() + 1, 0)}-${checkDay(i)}' class="today dayGet">${i}</div>`;
        } else {
            days += `<div class='dayGet' data-goto='${year}-${checkMoth(date.getMonth() + 1, 0)}-${checkDay(i)}'>${i}</div>`;
        }
    }

    for (let j = 1; j <= nextDays; j++) {
        days += `<div data-goto='${year + 1}-${checkMoth(date.getMonth()+1, 2)}-${checkDay(j)}' class="next-date dayGet">${j}</div>`;
        monthDays.innerHTML = days;
    }

    function checkMoth(moth, state) {

        if (parseInt(state) === 1) {
            if (parseInt(moth) === 0) {
                return 12;
            } else {
                if (parseInt(moth) > 0 && parseInt(moth) < 10) {
                    return "0" + moth
                }
            }
        } else if (parseInt(state) === 0) {
            if (parseInt(moth) < 10) {
                return "0" + moth
            }
            return moth;
        } else if (parseInt(state) === 2) {
            if (parseInt(moth) < 10) {
                return "0" + (moth + 1)
            }
        }
    }

    function checkDay(day) {
        if (parseInt(day) < 10) {
            return "0" + day
        }
        return day;
    }
    $('.days > div').click(function () {
        $("#calendar").fullCalendar('gotoDate', $(this).attr("data-goto"));
        $('#calendar').fullCalendar('changeView', 'agendaDay');
    })
};
renderCalendar();
document.querySelector(".prev-moth").addEventListener("click", () => {
    date.setMonth(date.getMonth() - 1);
    const dateChange = date.getMonth() + 1
    if (dateChange === 12)
        year = (year - 1);
    renderCalendar();
});

document.querySelector(".next-moth").addEventListener("click", () => {
    date.setMonth(date.getMonth() + 1);
    const dateChange = date.getMonth() + 1
    if (dateChange === 1)
        year = (year + 1);

    renderCalendar();
});


