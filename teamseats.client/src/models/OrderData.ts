import { Status } from "./Status";

export interface OrderData {
    id: number;
    isOwnedByUser: boolean;
    hasItemInOrder: boolean;
    authorName: string;
    authorPhoto: string;
    deliveryCost: number;
    restaurant: string;
    status: Status;
    closingTime: Date;
}
