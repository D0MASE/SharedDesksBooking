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

  //To handle screen is visible: "grid" or "profile"
  const [view, setView] = useState("grid");

  // To store data returned from ProfileController
  const [profileData, setProfileData] = useState(null);


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

  // Connects to the ProfileController to fetch user profile data
  const fetchProfile = async () => {
    if (!user.firstName || !user.lastName) {
      alert("Please enter your first and last name to view profile.");
      return;
    }
    
    try {
      const response = await axios.get(`https://localhost:7277/api/profile?firstName=${user.firstName}&lastName=${user.lastName}`);
      setProfileData(response.data);
      setView("profile");
    }
    catch (error) {
      console.error("Error fetching profile:", error);
      alert("Failed to fetch profile data.");
    }
  };

  // Load desks whenever the selected date changes
  useEffect(() => {
    loadDesks(selectedDate);
  }, [selectedDate]);

  return (
    <div className="container mt-4 pb-5">
      {/* --- NAVIGATION BAR --- */}
      <nav className="d-flex justify-content-between align-items-center mb-4 p-3 bg-white shadow-sm rounded border">
        <h2 className="m-0 text-primary">Office Booking</h2>
        <div>
          <button 
            className={`btn ${view === 'grid' ? 'btn-primary' : 'btn-outline-primary'} me-2`}
            onClick={() => setView("grid")}
          >
            Desks Grid
          </button>
          <button 
            className={`btn ${view === 'profile' ? 'btn-primary' : 'btn-outline-primary'}`}
            onClick={fetchProfile}
          >
            My Profile
          </button>
        </div>
      </nav>

      {view === "grid" ? (
        /* --- VIEW 1: DESK GRID --- */
        <div className="animate-fade-in">
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

          {/* --- MAIN DATE SELECTOR --- */}
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
              const statusClass = desk.status === "UnderMaintenance" ? 'secondary' : desk.reservation ? 'danger' : 'success';
              const isMine = desk.reservation &&
                desk.reservation.firstName.toLowerCase() === user.firstName.toLowerCase() &&
                desk.reservation.lastName.toLowerCase() === user.lastName.toLowerCase();

              return (
                <div key={desk.id} className="col-6 col-md-4 col-lg-3 text-center">
                  <div 
                    className={`card h-100 border-${statusClass} shadow-sm`}
                    title={desk.reservation 
                      ? `Reserved by: ${desk.reservation.firstName} ${desk.reservation.lastName}` 
                      : desk.status === "UnderMaintenance" ? "Under Maintenance" : "Available"}
                  >
                    <div className={`card-header bg-${statusClass} text-white fw-bold py-3`}>
                      Desk {desk.number}
                    </div>

                    <div className="card-body d-flex flex-column align-items-center justify-content-center py-4">
                      {isMine ? (
                        <div className="d-grid gap-2 w-100">
                          <small className="fw-bold text-danger text-center">Your Booking</small>
                          <button onClick={() => handleCancel(desk.reservation.id, true)} className="btn btn-danger btn-sm">Cancel today</button>
                          <button onClick={() => handleCancel(desk.reservation.id, false)} className="btn btn-outline-danger btn-sm">Cancel all</button>
                        </div>
                      ) : (
                        !desk.reservation && desk.status !== "UnderMaintenance" && (
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
                      {(desk.reservation && !isMine) || desk.status === "UnderMaintenance" ? (
                        <span className="text-muted small">
                          {desk.status === "UnderMaintenance" ? "Under Maintenance" : "Not Available"}
                        </span>
                      ) : null}
                    </div>
                  </div>
                </div>
              );
            })}
          </div>
        </div>
      ) : (
        /* --- VIEW 2: PROFILE PAGE --- */
        !profileData ? (
        <div className="text-center mt-5">
          <div className="spinner-border text-primary" role="status"></div>
          <p className="mt-2">Loading your profile...</p>
        </div>
        ) : (
        <div className="card shadow-sm p-4 animate-fade-in bg-white">
          <h3 className="text-center mb-4 border-bottom pb-3">
            {profileData?.firstName} {profileData?.lastName}'s History
          </h3>
          <div className="row">
            {/* Current/Active Bookings */}
            <div className="col-md-6">
              <h5 className="text-success border-bottom pb-2">Active Reservations</h5>
              {profileData?.currentReservations?.length > 0 ? (
                <div className="list-group">
                  {profileData.currentReservations.map(res => (
                    <div key={res.id} className="list-group-item border-start-0 border-end-0">
                      <strong>Desk {res.number}</strong>
                      <div className="text-muted small">
                        {new Date(res.startDate).toLocaleDateString()} - {new Date(res.endDate).toLocaleDateString()}
                      </div>
                    </div>
                  ))}
                </div>
              ) : <p className="text-muted">No active bookings found.</p>}
            </div>

            {/* Past Bookings */}
            <div className="col-md-6 mt-4 mt-md-0">
              <h5 className="text-secondary border-bottom pb-2">Past History</h5>
              {profileData?.pastReservations.length > 0 ? (
                <div className="list-group">
                  {profileData.pastReservations.map(res => (
                    <div key={res.id} className="list-group-item list-group-item-light border-start-0 border-end-0">
                      <strong>Desk {res.number}</strong>
                      <div className="text-muted small">
                        {new Date(res.startDate).toLocaleDateString()} - {new Date(res.endDate).toLocaleDateString()}
                      </div>
                    </div>
                  ))}
                </div>
              ) : <p className="text-muted">No past reservations found.</p>}
            </div>
          </div>
        </div>
        )
      )}
    </div>
  );
}

export default App;