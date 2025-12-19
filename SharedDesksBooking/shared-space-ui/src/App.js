import React, { useEffect, useState } from "react";
import axios from "axios";
import './App.css';
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import 'bootstrap/dist/css/bootstrap.min.css';

function App() {
  // --- STATE MANAGEMENT ---
  const [desks, setDesks] = useState([]); // Stores desks and their current reservations
  const [selectedDate, setSelectedDate] = useState(new Date()); // Date selected in the calendar
  const [user, setUser] = useState({ firstName: "", lastName: "" }); // Current user's info

  //  States for the Reservation Range Picker
  const [bookingDeskId, setBookingDeskId] = useState(null); // Tracks which desk is being booked
  const [startDate, setStartDate] = useState(new Date());
  const [endDate, setEndDate] = useState(new Date());

  // --- API CALLS ---
  
  // Fetch desks and their status for the selected date
  const loadDesks = async (date) => {
    try {
      const formattedDate = date.toLocaleDateString('en-CA');
      const response = await axios.get(`https://localhost:7277/api/desks?date=${formattedDate}`);
      setDesks(response.data);
    } catch (error) {
      console.error("Error: Could not fetch desks.");
    }
  };

  // Connects to the DELETE method with ReservationController
  const handleCancel = async (reservationId, onlyToday) => {
    const message = onlyToday 
      ? "Are you sure you want to cancel for today only?" 
      : "Are you sure you want to cancel the entire reservation range?";

    if (!window.confirm(message)) return;

    try {
      const formattedDate = selectedDate.toLocaleDateString('en-CA');
      await axios.delete(`https://localhost:7277/api/reservations/${reservationId}?onlyToday=${onlyToday}&date=${formattedDate}`);
      
      loadDesks(selectedDate);
    } catch (error) {
      console.error("Error cancelling reservation:", error);
      alert("Failed to cancel reservation.");
    }
  };

  // Connects to the POST method with ReservationController
  const handleReserve = async (deskId) => {
    if (!user.firstName || !user.lastName) {
      alert("Please enter your first and last name to make a reservation.");
      return;
    }

    try { 
      await axios.post(`https://localhost:7277/api/reservations`, {
        deskId: deskId,
        firstName: user.firstName,
        lastName: user.lastName,
        startDate: startDate.toLocaleDateString('en-CA'),
        endDate: (endDate || startDate).toLocaleDateString('en-CA')
      });

      setBookingDeskId(null); // Close the booking picker
      loadDesks(selectedDate);
    } catch (error) {
      alert(error.response?.data || "Booking failed");
    }
  };

  useEffect(() => {
    loadDesks(selectedDate);
  }, [selectedDate]);

  return (
    <div className="container mt-5 pb-5">
      <h1 className="text-center mb-4">Shared Desks Booking</h1>

      {/* --- USER IDENTIFICATION SECTION --- */}
      <div className="row justify-content-center mb-4">
        <div className="col-md-6 p-4 shadow-sm bg-light rounded border text-center">
          <h5 className="mb-3">Identify Yourself</h5>
          <div className="row g-2">
            <div className="col">
              <input 
                type="text" 
                className="form-control" 
                placeholder="First Name" 
                value={user.firstName}
                onChange={(e) => setUser({...user, firstName: e.target.value})}
              />
            </div>
            <div className="col">
              <input 
                type="text" 
                className="form-control" 
                placeholder="Last Name" 
                value={user.lastName}
                onChange={(e) => setUser({...user, lastName: e.target.value})}
              />
            </div>
          </div>
        </div>
      </div>

      {/* --- MAIN DATE SELECTOR  --- */}
      <div className="row justify-content-center mb-5">
        <div className="col-md-4 p-4 shadow-sm bg-light rounded text-center border">
          <label className="form-label fw-bold d-block">Select View Date:</label>
          <DatePicker 
            selected={selectedDate} 
            onChange={(date) => setSelectedDate(date)}
            dateFormat="yyyy-MM-dd"
            className="form-control text-center"
          />
        </div>
      </div>
      
      {/* --- DESK GRID SECTION --- */}
      <div className="row g-4">
        {desks.map((desk) => {
          const statusClass = desk.isUnderMaintenance ? 'secondary' : desk.reservation ? 'danger' : 'success';
          
          const isMine = desk.reservation &&
            desk.reservation.firstName.toLowerCase() === user.firstName.toLowerCase() &&
            desk.reservation.lastName.toLowerCase() === user.lastName.toLowerCase();

          return (
            <div key={desk.id} className="col-6 col-md-4 col-lg-3 text-center">
              <div 
                className={`card h-100 border-${statusClass} shadow-sm`}
                title={desk.reservation 
                  ? `Reserved by: ${desk.reservation.firstName} ${desk.reservation.lastName}` 
                  : desk.isUnderMaintenance ? "Under Maintenance" : "Available"}
              >
                <div className={`card-header bg-${statusClass} text-white fw-bold py-3`}>
                  Desk {desk.number}
                </div>

                <div className="card-body d-flex flex-column align-items-center justify-content-center py-4">
                  {/* Option 1: Reservation owned by current user  */}
                  {isMine ? (
                    <div className="d-grid gap-2 w-100">
                      <small className="fw-bold text-danger text-center">Your Booking</small>
                      <button onClick={() => handleCancel(desk.reservation.id, true)} className="btn btn-danger btn-sm">Cancel today</button>
                      <button onClick={() => handleCancel(desk.reservation.id, false)} className="btn btn-outline-danger btn-sm">Cancel all</button>
                    </div>
                  ) : (
                    /* Option 2: Desk is open - show reserve button or range picker  */
                    !desk.reservation && !desk.isUnderMaintenance && (
                      bookingDeskId === desk.id ? (
                        <div className="w-100">
                          <small className="fw-bold mb-1 d-block">Select Range:</small>
                          <DatePicker
                            selected={startDate}
                            onChange={(dates) => {
                              const [start, end] = dates;
                              setStartDate(start);
                              setEndDate(end);
                            }}
                            startDate={startDate}
                            endDate={endDate}
                            selectsRange
                            inline
                            minDate={new Date()}
                          />
                          <div className="d-flex gap-1 mt-2">
                            <button onClick={() => handleReserve(desk.id)} className="btn btn-success btn-sm w-100">Confirm</button>
                            <button onClick={() => setBookingDeskId(null)} className="btn btn-light btn-sm w-100 border">Cancel</button>
                          </div>
                        </div>
                      ) : (
                        <button onClick={() => {
                          setBookingDeskId(desk.id);
                          setStartDate(selectedDate);
                          setEndDate(selectedDate);
                        }} className="btn btn-success btn-sm px-4">Reserve</button>
                      )
                    )
                  )}
                  {/* Option 3: Reserved by others or Maintenance  */}
                  {(desk.reservation && !isMine) || desk.isUnderMaintenance ? (
                    <span className="text-muted small">
                      {desk.isUnderMaintenance ? "Under Maintenance" : "Not Available"}
                    </span>
                  ) : null}
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