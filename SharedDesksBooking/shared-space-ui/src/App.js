import React, { useEffect, useState } from "react";
import axios from "axios";
import './App.css';
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import 'bootstrap/dist/css/bootstrap.min.css';

function App() {
  const [desks, setDesks] = useState([]);
  const [selectedDate, setSelectedDate] = useState(new Date());

  const loadDesks = async (date) => {
    try {
      const formattedDate = date.toLocaleDateString('en-CA');
      const response = await axios.get(`https://localhost:7277/api/desks?date=${formattedDate}`);
      setDesks(response.data);
    } catch (error) {
      console.error("Klaida: Nepavyko pasiekti API.");
    }
  };

  useEffect(() => {
    loadDesks(selectedDate);
  }, [selectedDate]);

  return (
    <div className="container mt-5">
      <h1 className="text-center mb-4">Shared Desks Booking</h1>

      <div className="row justify-content-center mb-5">
        <div className="col-md-4 p-4 shadow-sm bg-light rounded text-center border">
          <label className="form-label fw-bold d-block">Pasirinkite dieną:</label>
          <DatePicker 
            selected={selectedDate} 
            onChange={(date) => setSelectedDate(date)}
            dateFormat="yyyy-MM-dd"
            className="form-control text-center"
          />
        </div>
      </div>
      
      <div className="row g-4">
        {desks.map((desk) => {
          const statusClass = desk.isUnderMaintenance ? 'secondary' : desk.reservation ? 'danger' : 'success';

          return (
            <div key={desk.id} className="col-6 col-md-4 col-lg-3">
              <div 
                className={`card h-100 border-${statusClass} shadow-sm`}
                title={desk.reservation 
                  ? `Rezervavo: ${desk.reservation.firstName} ${desk.reservation.lastName}` 
                  : desk.isUnderMaintenance ? "Vykdomi darbai" : "Laisva"}
              >
                <div className={`card-header bg-${statusClass} text-white text-center fw-bold`}>
                  Stalas {desk.number}
                </div>
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
}

export default App;