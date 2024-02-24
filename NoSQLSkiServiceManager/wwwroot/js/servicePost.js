document.addEventListener('DOMContentLoaded', function () {
    fillServiceTypesDropdown();
    fillPrioritiesDropdown();

    document.getElementById('serviceOrderForm').addEventListener('submit', function (event) {
        event.preventDefault();
        if (validateForm()) {
            createServiceOrder();
        }
    });

    document.getElementById('serviceTypeId').addEventListener('change', updateServiceDetails);
    document.getElementById('priorityId').addEventListener('change', updateServiceDetails);

    updateServiceDetails();
});

function fillServiceTypesDropdown() {
    const serviceTypes = [
        { Id: "1", Name: "Kleiner Service", Cost: 34.95 },
        { Id: "2", Name: "Grosser Service", Cost: 59.95 },
        { Id: "3", Name: "Rennski-Service", Cost: 74.95 },
        { Id: "4", Name: "Bindung montieren und einstellen", Cost: 24.95 },
        { Id: "5", Name: "Fell zuschneiden", Cost: 14.95 },
        { Id: "6", Name: "Heisswachsen", Cost: 19.95 }
    ];
    const dropdown = document.getElementById('serviceTypeId');
    serviceTypes.forEach(type => {
        const option = document.createElement('option');
        option.value = type.Id;
        option.textContent = `${type.Name} (${type.Cost.toFixed(2)} €)`;
        dropdown.appendChild(option);
    });
}

function fillPrioritiesDropdown() {
    const priorities = [
        { Id: "1", PriorityName: "Low", DayCount: 5 },
        { Id: "2", PriorityName: "Standard", DayCount: 0 },
        { Id: "3", PriorityName: "Express", DayCount: -2 }
    ];
    const dropdown = document.getElementById('priorityId');
    priorities.forEach(priority => {
        const option = document.createElement('option');
        option.value = priority.Id;
        option.textContent = priority.PriorityName;
        dropdown.appendChild(option);
    });
}

function validateForm() {
    let isValid = true;
    ['customerName', 'email', 'phoneNumber', 'serviceTypeId', 'priorityId'].forEach(id => {
        const input = document.getElementById(id);
        if (!input.value.trim()) {
            input.classList.add('is-invalid');
            isValid = false;
        } else {
            input.classList.remove('is-invalid');
        }
    });
    return isValid;
}

function createServiceOrder() {
    const serviceOrder = {
        customerName: document.getElementById('customerName').value,
        email: document.getElementById('email').value,
        phoneNumber: document.getElementById('phoneNumber').value,
        comments: document.getElementById('comments').value,
        serviceTypeId: document.getElementById('serviceTypeId').value,
        priorityId: document.getElementById('priorityId').value
    };

    fetch('https://localhost:7095/api/ServiceOrder', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', },
        body: JSON.stringify(serviceOrder)
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Netzwerkantwort war nicht ok');
            }
            return response.json();
        })
        .then(data => {
            console.log('Erfolg:', data);
            window.location.href = 'successPage.html';
        })
        .catch(error => {
            console.error('Fehler:', error);
            alert('Fehler beim Erstellen des Serviceauftrags. Bitte versuchen Sie es erneut.');
        });
}


function updateServiceDetails() {
    calculatePrice();
    calculatePickupDate();
}

function calculatePrice() {
    const serviceTypeId = document.getElementById('serviceTypeId').value;
    const priorityId = document.getElementById('priorityId').value;
    let baseCost;
    
    switch (serviceTypeId) {
        case "1": baseCost = 34.95; break;
        case "2": baseCost = 59.95; break;
        case "3": baseCost = 74.95; break;
        case "4": baseCost = 24.95; break;
        case "5": baseCost = 14.95; break;
        case "6": baseCost = 19.95; break;
        default: baseCost = 0;
    }
    
    let priceAdjustmentFactor = 1.0;
    if (priorityId === "3") { // Express
        priceAdjustmentFactor = 1.2;
    } else if (priorityId === "1") { // Low
        priceAdjustmentFactor = 0.8;
    }

    const calculatedPrice = baseCost * priceAdjustmentFactor;
    document.getElementById('calculatedPrice').textContent = `€ ${calculatedPrice.toFixed(2)}`;
}

function calculatePickupDate() {
    const priorityId = document.getElementById('priorityId').value;
    let dayCount = 0;
    
    switch (priorityId) {
        case "1": dayCount = 5; break; // Low
        case "2": dayCount = 0; break; // Standard
        case "3": dayCount = -2; break; // Express
        default: dayCount = 0;
    }

    const today = new Date();
    const pickupDate = new Date(today);
    pickupDate.setDate(today.getDate() + 7 + dayCount);

    document.getElementById('calculatedPickupDate').textContent = pickupDate.toLocaleDateString();
}