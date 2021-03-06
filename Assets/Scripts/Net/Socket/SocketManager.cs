﻿using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;

public class OUTPUT_ELEMENT
{
	public OUTPUT_ELEMENT(byte[] data, int dataSize, PACKET_TYPE type)
	{
		mData = data;
		mDataSize = dataSize;
		mType = type;
	}
	public byte[] mData;
	public int mDataSize;
	public PACKET_TYPE mType;
};

public class SEND_ELEMENT : OUTPUT_ELEMENT
{
	public SEND_ELEMENT(byte[] data, int dataSize, PACKET_TYPE type)
		:
		base(data, dataSize, type)
	{ }
}

public class INPUT_ELEMENT
{
	public INPUT_ELEMENT(PACKET_TYPE type, byte[] data)
	{
		mType = type;
		mData = data;
	}
	public PACKET_TYPE mType;
	public byte[] mData;
};

public class SocketManager : GameBase
{
	protected int mMaxReceiveCount;
	protected Socket mServerSocket;
	protected Thread mReceiveThread;
	protected Thread mSendThread;
	protected List<OUTPUT_ELEMENT> mOutputList;
	protected List<INPUT_ELEMENT> mRecieveList;
	protected List<SEND_ELEMENT> mSendList;
	protected SocketFactoryManager mSocketFactoryManager;
	protected bool mRun;
	protected int mHeartBeatTimes;
	protected float mHeartBeatTimeCount = 0.0f;
	protected float mHeartBeatMaxTime = 0.0f;
	public SocketManager()
	{
		mMaxReceiveCount = 1024;
		mOutputList = new List<OUTPUT_ELEMENT>();
		mRecieveList = new List<INPUT_ELEMENT>();
		mSendList = new List<SEND_ELEMENT>();
		mSocketFactoryManager = new SocketFactoryManager();
		mRun = true;
	}
	public void init()
	{
		mSocketFactoryManager.init();
		mHeartBeatMaxTime = mGameConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_HEART_BEAT_NITERVAL);
		// 创建socket  
		mServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		try
		{
			int port = (int)mGameConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_SOCKET_TCP_PORT);
			IPAddress serverIP = IPAddress.Parse(mGameConfig.getStringParam(GAME_DEFINE_STRING.GDS_TCP_SERVER_IP));
			mServerSocket.Connect(serverIP, port);
		}
		catch
		{
			UnityUtility.logInfo("can not connect server!");
			mServerSocket = null;
			return;
		}
		mSendThread = new Thread(sendSocket);
		mSendThread.Start();
		mReceiveThread = new Thread(receiveSocket);
		mReceiveThread.Start();
	}
	public void update(float elapsedTime)
	{
		if(mHeartBeatTimeCount >= 0.0f)
		{
			mHeartBeatTimeCount += elapsedTime;
			if(mHeartBeatTimeCount >= mHeartBeatMaxTime)
			{
				heartBeat();
				// 停止计时,等待服务器确认心跳后再开始计时
				mHeartBeatTimeCount = -1.0f;
			}
		}
		processInput();
		processOutput();
	}
	public void destroy()
	{
		mRun = false;
		if (mSendThread != null)
		{
			mSendThread.Abort();
			mSendThread = null;
		}
		if (mReceiveThread != null)
		{
			mReceiveThread.Abort();
			mReceiveThread = null;
		}
		if (mServerSocket != null)
		{
			mServerSocket.Close();
			mServerSocket = null;
		}
	}
	public SocketPacket createPacket(PACKET_TYPE type)
	{
		return mSocketFactoryManager.createPacket(type);
	}
	public void sendMessage(SocketPacket packet)
	{
		// 将消息包中的数据准备好,然后放入发送列表中
		// 前四个字节分别是两个short,代表消息类型和消息内容长度
		byte[] packetData = new byte[sizeof(short) + sizeof(short) + packet.getSize()];
		int index = 0;
		// 消息类型
		BinaryUtility.writeShort(packetData, ref index, (short)(packet.getPacketType()));
		// 消息长度
		BinaryUtility.writeShort(packetData, ref index, (short)(packet.getSize()));
		if(packet.getSize() > 0)
		{
			byte[] realData = new byte[packet.getSize()];
			packet.write(realData);
			// 消息内容
			BinaryUtility.writeBytes(packetData, ref index, realData);
		}
		lock (mOutputList)
		{
			mOutputList.Add(new OUTPUT_ELEMENT(packetData, packet.getSize(), packet.getPacketType()));
		}
	}
	public void notifyHeartBeatRet(int heartBeatTimes)
	{
		if(heartBeatTimes != mHeartBeatTimes)
		{
			UnityUtility.logError("心跳错误!");
		}
		else
		{
			mHeartBeatTimeCount = 0.0f;
		}
	}
	//-------------------------------------------------------------------------------------------------------------------------
	// 处理接收到的所有消息
	protected void processInput()
	{
		// 等待解锁接收流的读写,并锁定接收流
		lock (mRecieveList)
		{
			int receiveCount = mRecieveList.Count;
			for (int i = 0; i < receiveCount; ++i)
			{
				INPUT_ELEMENT element = mRecieveList[i];
				SocketPacket packetReply = createPacket(element.mType);
				packetReply.read(element.mData);
				packetReply.execute();
			}
			mRecieveList.Clear();
		}
	}
	// 发送这一帧中所有的消息
	protected void processOutput()
	{
		;
	}
	// 发送Socket消息
	protected void sendSocket()
	{
		while (mRun)
		{
			lock(mOutputList)
			{
				int dataCount = mOutputList.Count;
				for (int i = 0; i < dataCount; ++i)
				{
					mServerSocket.Send(mOutputList[i].mData);
					PACKET_TYPE type = mOutputList[i].mType;
					UnityUtility.logInfo("send socket : type : " + type + ", size : " + mOutputList[i].mDataSize);
				}
				mOutputList.Clear();
			}
			Thread.Sleep(10);
		}
	}
	// 接收Socket消息
	protected void receiveSocket()
	{
		IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
		EndPoint ep = (EndPoint)endpoint;
		while (mRun)
		{
			byte[] recvBuff = new byte[mMaxReceiveCount];
			int nRecv = mServerSocket.ReceiveFrom(recvBuff, ref ep);
			if (nRecv < 0)
			{
				UnityUtility.logInfo("网络连接中断!");
				return;
			}
			else if (nRecv == 0)
			{
				UnityUtility.logInfo("已与服务器断开连接!");
				return;
			}
			int index = 0;
			while (true)
			{
				if (index + sizeof(short) > nRecv)
				{
					break;
				}
				// 读取包类型(short)
				PACKET_TYPE type = (PACKET_TYPE)BinaryUtility.readShort(recvBuff, ref index);
				// 客户端接收到的必须是SC类型的
				if (type <= PACKET_TYPE.PT_SC_MIN || type >= PACKET_TYPE.PT_SC_MAX)
				{
					UnityUtility.logInfo("packet type error : " + type);
					break;
				}
				int packetSize = mSocketFactoryManager.getPacketSize(type);
				if (packetSize >= 0)
				{
					// 读取消息长度(short)
					short realDataSize = BinaryUtility.readShort(recvBuff, ref index);
					if(realDataSize != packetSize)
					{
						UnityUtility.logError("error : wrong packet size! type : " + type + "readed : " + realDataSize + ", packet size : " + packetSize, false);
						break;
					}
					if (packetSize > nRecv - sizeof(short))
					{
						UnityUtility.logError("error : wrong packet data! packet : " + type + ", need size : " + packetSize + ", receive size : " + (nRecv - sizeof(PACKET_TYPE)), false);
						break;
					}
					else
					{
						UnityUtility.logInfo("receive : client : " + endpoint.Address.ToString() + ", type : " + type + ", size : " + packetSize);
					}
					lock (mRecieveList)
					{
						if(packetSize != 0)
						{
							byte[] recvData = new byte[packetSize];
							// 读取消息内容(byte[])
							BinaryUtility.readBytes(recvBuff, ref index, recvData);
							mRecieveList.Add(new INPUT_ELEMENT(type, recvData));
						}
						else
						{
							byte[] recvData = null;
							mRecieveList.Add(new INPUT_ELEMENT(type, recvData));
						}
						UnityUtility.logInfo("receive : type : " + type + ", count : " + nRecv + ", client ip : " + endpoint.Address.ToString());
					}
					// 该段消息内存已经解析完了
					if (index == nRecv)
					{
						break;
					}
				}
				// 如果消息解析发生错误,则不再解析
				else
				{
					break;
				}
			}
			Thread.Sleep(10);
		}
	}
	protected void heartBeat()
	{
		CSHeartBeat beat = createPacket(PACKET_TYPE.PT_CS_HEART_BEAT) as CSHeartBeat;
		beat.setHeartBeatTimes(++mHeartBeatTimes);
		sendMessage(beat);
	}
}