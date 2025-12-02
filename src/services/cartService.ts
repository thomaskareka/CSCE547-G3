import { UUID } from "crypto";
import { CartItem, Booking } from "../models/cartItem";
import IPark from "../models/park";
export default class CartService {
    private items: CartItem[];

    private CART_KEY = 'rideFinderExampleApp'
    private KEY_NAME = 'cart_key';

    public CartItemToBooking(item: CartItem, cart_id: UUID) : Booking {
        return {
            adults: item.numAdults,
            children: item.numKids,
            cartId: cart_id,
            parkId: item.park.id as UUID,
            numDays: item.numDays,
            startDate: new Date(Date.now())
        }
    }

    public BookingToCartItem(item: Booking, park: IPark) : CartItem {
        return {
            park: park,
            numDays: item.numDays,
            numAdults: item.adults,
            numKids: item.children
        }
    }

    private GetCartId(): string {
        let id = localStorage.getItem(this.KEY_NAME);
        if (!id) {
            id = crypto.randomUUID();
            localStorage.setItem(this.KEY_NAME, id);
            localStorage.setItem(id, JSON.stringify([]));
        }
        return id;
    }
    //loadCart will be our public facing method, all invocations of getCart should be internal so we only have one source of truth

    loadCart = async (): Promise<CartItem[]> => {
        const id = this.GetCartId();
        console.log(id);
        const res = await fetch(`${process.env.REACT_APP_API_URL}/api/Cart/${id}`);
        const data = await res.json();

        var items = data.outCart.items
        const out: CartItem[] = items.map((item: any) => {
            return this.BookingToCartItem(item, item.park);
        });
        return out;
    }

    addItemToCart = async (newItem: CartItem): Promise<CartItem[]> => {
        const id = await this.GetCartId();

        const booking = this.CartItemToBooking(newItem, id as UUID)

        const res = await fetch(`${process.env.REACT_APP_API_URL}/add_booking`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                cartId: id,
                parkId: booking.parkId,
                bookingInfo: {
                    adults: booking.adults,
                    children: booking.children,
                    startDate: new Date(Date.now()),
                    numDays: booking.numDays
                }
            })
        });

        return this.loadCart();
    }

    removeItemFromCart = async (remItem: CartItem): Promise<CartItem[]> => {
        const cart = await this.loadCart();
        return cart;
    }

    updateCart = async (oldItem: CartItem, newItem: CartItem): Promise<CartItem[]> => {
        return
    }

    private save(cart: CartItem[]) {
        var id = localStorage.getItem("cart_key");
        localStorage.setItem(id, JSON.stringify(cart));
    }
}