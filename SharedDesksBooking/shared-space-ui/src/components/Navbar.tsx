import React from "react";
import { Link } from "react-router-dom";

interface NavbarProps {
    view: "grid" | "profile";
    onProfileClick: () => void;
}

const Navbar: React.FC<NavbarProps> = ({ view, onProfileClick }) => {
    return (
        <nav className="d-flex justify-content-between align-items-center mb-4 p-3 bg-white shadow-sm rounded border">
            <h2 className="m-0 text-primary">Office Booking</h2>
            <div>
                <Link
                    to="/"
                    className={`btn ${view === "grid" ? "btn-primary" : "btn-outline-primary"} me-2`}
                >
                    Desks Grid
                </Link>
                <button
                    className={`btn ${view === "profile" ? "btn-primary" : "btn-outline-primary"}`}
                    onClick={onProfileClick}
                >
                    My Profile
                </button>
            </div>
        </nav>
    );
};

export default Navbar;
