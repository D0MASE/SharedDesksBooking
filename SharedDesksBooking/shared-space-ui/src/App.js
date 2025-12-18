import React, { useEffect, useState} from "react";
import axios from "axios";
import './App.css';

function App() {
  const [desks, setDesks] = useState([]);

  const loadDesks = async () => {
    try {
      const response = await axios.get("https://localhost:7277/api/desks");
      setDesks(response.data);
    } catch (error) {
      console.error("Klaida: Nepavyko pasiekti API. Patikrinkite, ar Back-end veikia.");
    }
  };

  useEffect(() => {
    loadDesks();
  }, []);

  return (
    <div className="App">
      <h1>Shared Desks Booking</h1>
      <div className="desk-grid">
        {desks.map(desk => (
          <div 
            key={desk.id} 
            className={`desk-item ${desk.isUnderMaintenance ? 'gray' : desk.reservation ? 'red' : 'green'}`}
            title={desk.reservation ? `Rezervavo: ${desk.reservation.firstName} ${desk.reservation.lastName}` : 'Laisva'}
          >
            {desk.number}
          </div>
        ))}
      </div>
    </div>
  );
}

export default App;
